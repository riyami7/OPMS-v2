using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OperationalPlanMS.Data;
using OperationalPlanMS.Models;
using OperationalPlanMS.Models.Entities;
using OperationalPlanMS.Models.ViewModels;
using System.Security.Claims;

namespace OperationalPlanMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var userRoleStr = User.FindFirst(ClaimTypes.Role)?.Value;
            var roleNameAr = User.FindFirst("RoleNameAr")?.Value;
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var employeeRank = User.FindFirst("EmployeeRank")?.Value;
            int.TryParse(userIdStr, out int userId);
            Enum.TryParse<UserRole>(userRoleStr, out UserRole userRole);

            ViewBag.UserName = userName;
            ViewBag.UserRole = userRole;
            ViewBag.RoleNameAr = roleNameAr;
            ViewBag.EmployeeRank = employeeRank;

            if (userRole == UserRole.Admin || userRole == UserRole.SuperAdmin)
            {
                ViewBag.PendingApprovals = await _db.Steps.CountAsync(s => !s.IsDeleted && s.ApprovalStatus == ApprovalStatus.Pending);
            }
            else
            {
                ViewBag.PendingApprovals = 0;
            }

            try
            {
                var viewModel = new DashboardViewModel();

                if (userRole == UserRole.Admin || userRole == UserRole.SuperAdmin)
                    await LoadAdminDashboard(viewModel);
                else if (userRole == UserRole.Executive)
                    await LoadExecutiveDashboard(viewModel);
                else
                    await LoadMyDashboard(viewModel, userId);

                // الإشعارات الأخيرة — لكل المستخدمين
                viewModel.RecentNotifications = await _db.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحميل لوحة التحكم");
                ViewBag.DatabaseError = "حدث خطأ في الاتصال بقاعدة البيانات. يرجى المحاولة لاحقاً.";
                return View(new DashboardViewModel());
            }
        }

        #region Dashboard Loaders

        private async Task LoadAdminDashboard(DashboardViewModel model)
        {
            var initiatives = await _db.Initiatives.Where(i => !i.IsDeleted)
                .Include(i => i.Projects.Where(p => !p.IsDeleted))
                    .ThenInclude(p => p.Steps.Where(s => !s.IsDeleted))
                .ToListAsync();

            // استخراج المشاريع والخطوات من المبادرات (نفس طريقة التقارير)
            var projects = initiatives.SelectMany(i => i.Projects).ToList();
            var steps = projects.SelectMany(p => p.Steps).ToList();

            model.TotalInitiatives = initiatives.Count;
            model.TotalProjects = projects.Count;
            model.TotalSteps = steps.Count;
            model.TotalUsers = await _db.Users.CountAsync(u => u.IsActive);

            model.CompletedInitiatives = initiatives.Count(i =>
                i.Projects.Any() && i.Projects.All(p => p.ProgressPercentage >= 100));
            model.InProgressInitiatives = initiatives.Count(i =>
                i.Projects.Any(p => p.ProgressPercentage > 0 && p.ProgressPercentage < 100));
            model.DelayedInitiatives = initiatives.Count(i =>
                i.Projects.Any(p => IsProjectDelayed(p)));
            model.CompletedProjects = projects.Count(p => p.ProgressPercentage >= 100);
            model.InProgressProjects = projects.Count(p => p.ProgressPercentage > 0 && p.ProgressPercentage < 100 && !IsProjectDelayed(p));
            model.DelayedProjects = projects.Count(p => IsProjectDelayed(p));

            model.AverageInitiativeProgress = initiatives.Any() ? Math.Round(initiatives.Average(i => i.ProgressPercentage), 1) : 0;
            model.AverageProjectProgress = projects.Any() ? Math.Round(projects.Average(p => p.ProgressPercentage), 1) : 0;

            model.RecentInitiatives = initiatives.OrderByDescending(i => i.CreatedAt).Take(5).ToList();
            model.RecentProjects = projects.OrderByDescending(p => p.CreatedAt).Take(5).ToList();
            model.OverdueProjects = projects.Where(p => IsProjectDelayed(p))
                .OrderBy(p => p.PlannedEndDate).Take(5).ToList();
        }

        private async Task LoadExecutiveDashboard(DashboardViewModel model)
        {
            await LoadAdminDashboard(model);
            model.TotalUsers = 0; // Executive ما يشوف عدد المستخدمين
        }

        /// <summary>
        /// لوحة تحكم شخصية — تعرض كل ما المستخدم معيّن عليه بغض النظر عن دوره
        /// </summary>
        private async Task LoadMyDashboard(DashboardViewModel model, int userId)
        {
            var empNumber = await _db.Users.Where(u => u.Id == userId)
                .Select(u => u.ADUsername).FirstOrDefaultAsync() ?? "";

            // === جمع كل المشاريع المرتبطة بالمستخدم ===

            // 1. مشاريع أنا مديرها
            var managedProjectIds = await _db.Projects
                .Where(p => !p.IsDeleted && p.ProjectManagerId == userId)
                .Select(p => p.Id).ToListAsync();

            // 2. مشاريع أنا مساعد مديرها
            var deputyProjectIds = !string.IsNullOrEmpty(empNumber)
                ? await _db.Projects.Where(p => !p.IsDeleted && p.DeputyManagerEmpNumber == empNumber)
                    .Select(p => p.Id).ToListAsync()
                : new List<int>();

            // 3. مشاريع فيها خطوات معيّنة لي
            var stepProjectIds = await _db.Steps
                .Where(s => !s.IsDeleted && s.AssignedToId == userId)
                .Select(s => s.ProjectId).Distinct().ToListAsync();

            // 4. مبادرات أنا مشرفها
            var supervisedInitiativeIds = await _db.Initiatives
                .Where(i => !i.IsDeleted && i.SupervisorId == userId)
                .Select(i => i.Id).ToListAsync();

            // 5. مبادرات عبر InitiativeAccess
            var accessInitiativeIds = await _db.InitiativeAccess
                .Where(a => a.UserId == userId && a.IsActive)
                .Select(a => a.InitiativeId).ToListAsync();

            // === تجميع كل الـ IDs ===
            var allMyProjectIds = managedProjectIds
                .Union(deputyProjectIds)
                .Union(stepProjectIds)
                .Distinct().ToList();

            var allMyInitiativeIds = supervisedInitiativeIds
                .Union(accessInitiativeIds)
                .Distinct().ToList();

            // أضف مبادرات المشاريع
            var projectInitiativeIds = await _db.Projects
                .Where(p => allMyProjectIds.Contains(p.Id))
                .Select(p => p.InitiativeId).Distinct().ToListAsync();
            allMyInitiativeIds = allMyInitiativeIds.Union(projectInitiativeIds).Distinct().ToList();

            // === تحميل البيانات ===
            var myInitiatives = await _db.Initiatives
                .Where(i => !i.IsDeleted && allMyInitiativeIds.Contains(i.Id))
                .Include(i => i.Projects.Where(p => !p.IsDeleted))
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            var myProjects = await _db.Projects
                .Where(p => !p.IsDeleted && allMyProjectIds.Contains(p.Id))
                .Include(p => p.Initiative)
                .Include(p => p.Steps.Where(s => !s.IsDeleted))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var mySteps = await _db.Steps
                .Where(s => !s.IsDeleted && s.AssignedToId == userId)
                .Include(s => s.Project)
                .OrderBy(s => s.PlannedEndDate)
                .ToListAsync();

            // === الإحصائيات ===
            model.TotalInitiatives = myInitiatives.Count;
            model.TotalProjects = myProjects.Count;
            model.TotalSteps = mySteps.Count;

            model.CompletedInitiatives = myInitiatives.Count(i =>
                i.Projects.Any() && i.Projects.All(p => p.ProgressPercentage >= 100));
            model.InProgressInitiatives = myInitiatives.Count(i =>
                i.Projects.Any(p => p.ProgressPercentage > 0 && p.ProgressPercentage < 100));
            model.DelayedInitiatives = myInitiatives.Count(i =>
                i.Projects.Any(p => IsProjectDelayed(p)));

            model.CompletedProjects = myProjects.Count(p => p.ProgressPercentage >= 100);
            model.InProgressProjects = myProjects.Count(p => p.ProgressPercentage > 0 && p.ProgressPercentage < 100 && !IsProjectDelayed(p));
            model.DelayedProjects = myProjects.Count(p => IsProjectDelayed(p));

            model.CompletedSteps = mySteps.Count(s => s.ProgressPercentage >= 100);
            model.InProgressSteps = mySteps.Count(s => s.ProgressPercentage > 0 && s.ProgressPercentage < 100 && !IsStepDelayed(s));
            model.DelayedSteps = mySteps.Count(s => IsStepDelayed(s));

            model.AverageInitiativeProgress = myInitiatives.Any()
                ? Math.Round(myInitiatives.Average(i => i.ProgressPercentage), 1) : 0;
            model.AverageProjectProgress = myProjects.Any()
                ? Math.Round(myProjects.Average(p => p.ProgressPercentage), 1) : 0;

            // === القوائم ===
            model.RecentInitiatives = myInitiatives.Take(5).ToList();
            model.RecentProjects = myProjects.Take(5).ToList();
            model.MySteps = mySteps.Where(s => s.Status != StepStatus.Completed).Take(10).ToList();
            model.OverdueProjects = myProjects
                .Where(p => IsProjectDelayed(p))
                .OrderBy(p => p.PlannedEndDate).Take(5).ToList();

            // خطوات متأخرة
            model.OverdueSteps = mySteps
                .Where(s => IsStepDelayed(s))
                .OrderBy(s => s.PlannedEndDate).Take(5).ToList();

            // مواعيد قادمة (خطوات خلال 7 أيام)
            model.MyUpcomingDeadlines = mySteps
                .Where(s => s.Status != StepStatus.Completed
                    && s.PlannedEndDate >= DateTime.Today
                    && s.PlannedEndDate <= DateTime.Today.AddDays(7))
                .OrderBy(s => s.PlannedEndDate).Take(5).ToList();
        }

        #endregion

        /// <summary>
        /// الصفحة الرئيسية للخطة الاستراتيجية
        /// </summary>
        [AllowAnonymous]
        public async Task<IActionResult> StrategicOverview()
        {
            var viewModel = new StrategicOverviewViewModel
            {
                SystemSettings = await _db.SystemSettings.FirstOrDefaultAsync(),
                UnitSettings = await _db.OrganizationalUnitSettings.ToListAsync(),
                Axes = await _db.StrategicAxes.Where(a => a.IsActive).OrderBy(a => a.OrderIndex).ToListAsync(),
                StrategicObjectives = await _db.StrategicObjectives.Where(s => s.IsActive).OrderBy(s => s.OrderIndex).ToListAsync(),
                MainObjectives = await _db.MainObjectives.Where(m => m.IsActive).OrderBy(m => m.OrderIndex).ToListAsync(),
                SubObjectives = await _db.SubObjectives.Where(s => s.IsActive).OrderBy(s => s.OrderIndex).ToListAsync(),
                CoreValues = await _db.CoreValues.Where(v => v.IsActive).OrderBy(v => v.OrderIndex).ToListAsync()
            };
            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> VisionMission()
        {
            var viewModel = new StrategicOverviewViewModel
            {
                SystemSettings = await _db.SystemSettings.FirstOrDefaultAsync(),
                UnitSettings = await _db.OrganizationalUnitSettings.ToListAsync()
            };
            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> StrategicObjectives()
        {
            var viewModel = new StrategicOverviewViewModel
            {
                Axes = await _db.StrategicAxes.Where(a => a.IsActive).OrderBy(a => a.OrderIndex).ToListAsync(),
                StrategicObjectives = await _db.StrategicObjectives.Where(s => s.IsActive).OrderBy(s => s.OrderIndex).ToListAsync(),
                MainObjectives = await _db.MainObjectives.Where(m => m.IsActive).OrderBy(m => m.OrderIndex).ToListAsync(),
                SubObjectives = await _db.SubObjectives.Where(s => s.IsActive).OrderBy(s => s.OrderIndex).ToListAsync()
            };
            return View(viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> CoreValues()
        {
            var viewModel = new StrategicOverviewViewModel
            {
                CoreValues = await _db.CoreValues.Where(v => v.IsActive).OrderBy(v => v.OrderIndex).ToListAsync()
            };
            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        #region Helper Methods

        private bool IsStepDelayed(Step step)
        {
            if (step.ProgressPercentage >= 100) return false;
            if (step.Status == StepStatus.Cancelled) return false;
            if (step.ActualEndDate.HasValue && step.ActualEndDate.Value < DateTime.Today) return true;
            return step.Status == StepStatus.Delayed;
        }

        private bool IsProjectDelayed(Project project)
        {
            if (project.ProgressPercentage >= 100) return false;
            if (project.Steps != null && project.Steps.Any(s => !s.IsDeleted && IsStepDelayed(s))) return true;
            if (project.ActualEndDate.HasValue && project.ActualEndDate.Value < DateTime.Today) return true;
            return false;
        }

        #endregion
    }
}
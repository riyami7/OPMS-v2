using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MOD.OPMS.HttpApi.ExternalApiClients.Jund;
using OperationalPlanMS.Data;
using OperationalPlanMS.Models.ViewModels;
using OperationalPlanMS.Services;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Claims;

namespace OperationalPlanMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountController> _logger;
        private readonly IExternalApiService _externalApiService;
        private readonly IWebHostEnvironment _env;
        private readonly IJundClient _JundClient;

        // تشفير كلمات المرور
        private static readonly Microsoft.AspNetCore.Identity.PasswordHasher<Models.Entities.User> _passwordHasher = new();

        public AccountController(
            AppDbContext db,
            IConfiguration config,
            ILogger<AccountController> logger,
            IExternalApiService externalApiService,
            IWebHostEnvironment env,
            IJundClient jundClient)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _externalApiService = externalApiService;
            _env = env;
            _JundClient = jundClient;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.ADUsername == model.Username && u.IsActive);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "رقم الموظف غير موجود أو الحساب غير نشط");
                return View(model);
            }

            bool isAuthenticated = false;
            var adEnabled = _config.GetValue<bool>("ActiveDirectory:Enabled");

            if (adEnabled)
            {
                // AD Authentication الحقيقي
                isAuthenticated = ValidateWithActiveDirectory(model.Username, model.Password);
            }
            else
            {
                // Fallback للتطوير - التحقق من كلمة المرور المخزنة
                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    try
                    {
                        // أولاً: محاولة التحقق كـ hash مشفر
                        var verifyResult = _passwordHasher.VerifyHashedPassword(
                            user, user.PasswordHash, model.Password);

                        if (verifyResult == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success
                            || verifyResult == Microsoft.AspNetCore.Identity.PasswordVerificationResult.SuccessRehashNeeded)
                        {
                            isAuthenticated = true;

                            // إعادة تشفير إذا الخوارزمية قديمة
                            if (verifyResult == Microsoft.AspNetCore.Identity.PasswordVerificationResult.SuccessRehashNeeded)
                            {
                                user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                                await _db.SaveChangesAsync();
                            }
                        }
                    }
                    catch (FormatException)
                    {
                        // PasswordHash ليس hash صالح — يعني مخزن كنص عادي
                    }

                    // Fallback: إذا ما تم التحقق بعد — مقارنة كنص عادي
                    if (!isAuthenticated && user.PasswordHash == model.Password)
                    {
                        // كلمة مرور قديمة (نص عادي) — تحويلها تلقائياً لـ hash
                        isAuthenticated = true;
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
                        await _db.SaveChangesAsync();
                        _logger.LogInformation("تم تحويل كلمة مرور المستخدم {Username} من نص عادي إلى hash", user.ADUsername);
                    }
                }
            }

            if (!isAuthenticated)
            {
                ModelState.AddModelError(string.Empty, "اسم المستخدم او كلمة المرور غير صحيحة");
                return View(model);
            }

            // === جلب صورة الموظف من API الخارجي (إذا لم تكن موجودة أو قديمة) ===
            await SyncProfilePhotoAsync(user);

            await SignInUser(user, model.RememberMe);

            user.LastLoginAt = DateTime.Now;
            await _db.SaveChangesAsync();

            // ربط المشاريع والخطوات التي عُيِّن عليها هذا الموظف قبل إنشاء حسابه
            await SyncUserAssignments(user.Id, user.ADUsername);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Home");
        }

        private bool ValidateWithActiveDirectory(string username, string password)
        {
            try
            {
                var domain = _config["ActiveDirectory:Domain"] ?? "";
                var ldapPath = _config["ActiveDirectory:LdapPath"] ?? "";

                // Just test the credentials - this is the most reliable method
                using var entry = new System.DirectoryServices.DirectoryEntry(ldapPath, $"{domain}\\{username}", password);

                // If we reach here, authentication was successful
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AD Authentication Error: {ex.Message}");
                return false;
            }
        }

        private async Task SignInUser(Models.Entities.User user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullNameAr),
                new Claim("FullNameAr", user.FullNameAr),
                new Claim("FullNameEn", user.FullNameEn ?? user.FullNameAr),
                new Claim(ClaimTypes.Role, ((Models.UserRole)user.RoleId).ToString()),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim("RoleNameAr", user.Role?.NameAr ?? ""),
                new Claim("RoleNameEn", user.Role?.NameEn ?? ""),
                new Claim("IsStepApprover", user.IsStepApprover.ToString()),
                new Claim("EmployeeRank", user.EmployeeRank ?? ""),
            };

            if (user.ExternalUnitId.HasValue)
                claims.Add(new Claim("ExternalUnitId", user.ExternalUnitId.Value.ToString()));

            if (!string.IsNullOrEmpty(user.ExternalUnitName))
                claims.Add(new Claim("UnitName", user.ExternalUnitName));

            if (!string.IsNullOrEmpty(user.EmployeePosition))
                claims.Add(new Claim("EmployeePosition", user.EmployeePosition));

            if (user.LastLoginAt.HasValue)
                claims.Add(new Claim("LastLoginAt", user.LastLoginAt.Value.ToString()));


            // Multi-Tenancy: إضافة TenantId إلى الـ claims
            if (user.TenantId.HasValue)
                claims.Add(new Claim("TenantId", user.TenantId.Value.ToString()));

            if (!string.IsNullOrEmpty(user.Email))
                claims.Add(new Claim(ClaimTypes.Email, user.Email));

            if (!string.IsNullOrEmpty(user.ProfileImage))
                claims.Add(new Claim("ProfileImage", user.ProfileImage));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        /// <summary>
        /// عند تسجيل الدخول — يربط المشاريع والخطوات التي عُيِّن عليها الموظف
        /// قبل إنشاء حسابه في النظام (AssignedToId / ProjectManagerId كانت null)
        /// </summary>
        private async Task SyncUserAssignments(int userId, string empNumber)
        {
            try
            {
                // ربط مشاريع لم تُربط بعد
                await _db.Projects
                    .Where(p => p.ProjectManagerEmpNumber == empNumber
                             && p.ProjectManagerId == null
                             && !p.IsDeleted)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.ProjectManagerId, userId));

                // ربط خطوات لم تُربط بعد
                await _db.Steps
                    .Where(s => s.AssignedToEmpNumber == empNumber
                             && s.AssignedToId == null
                             && !s.IsDeleted)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.AssignedToId, userId));
            }
            catch
            {
                // لا نوقف تسجيل الدخول إذا فشل الـ sync
            }
        }

        /// <summary>
        /// جلب صورة الموظف من API الخارجي وحفظها محلياً
        /// </summary>
        private async Task SyncProfilePhotoAsync(Models.Entities.User user)
        {
            try
            {
                // تخطّي إذا الصورة موجودة وحديثة (أقل من 30 يوم)
                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    var localPath = Path.Combine(_env.WebRootPath, user.ProfileImage.TrimStart('/'));
                    if (System.IO.File.Exists(localPath))
                    {
                        var fileAge = DateTime.Now - new FileInfo(localPath).LastWriteTime;
                        if (fileAge.TotalDays < 30) return; // الصورة حديثة
                    }
                }

                // جلب الصورة من API
                var employee = await _JundClient.GetEmployeeAsync(user.ADUsername);
                if (employee?.Photo == null || employee.Photo.Length == 0) return;

                // حفظ محلياً
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "profiles");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"profile_{user.Id}_api.jpg";
                var filePath = Path.Combine(uploadsFolder, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, employee.Photo);

                // تحديث المسار في DB
                user.ProfileImage = $"/uploads/profiles/{fileName}";
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                // فشل جلب الصورة — نتجاهله ونكمل عرض الملف الشخصي بدون صورة
            }
        }

    }
}
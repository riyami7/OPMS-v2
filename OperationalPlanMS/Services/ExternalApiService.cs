using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MOD.OPMS.HttpApi.ExternalApiClients.Jund;
using MOD.OPMS.HttpApi.ExternalApiClients.Jund.Dto;
using OperationalPlanMS.Data;
using OperationalPlanMS.Models.ExternalApi;

// Alias للتفريق بين الـ Entity والـ DTO
using ExternalUnitEntity = OperationalPlanMS.Models.Entities.ExternalOrganizationalUnit;
namespace OperationalPlanMS.Services
{
    /// <summary>
    /// خدمة التعامل مع API نظام الموارد البشرية الخارجي
    /// </summary>
    public interface IExternalApiService
    {
        // مزامنة مع قاعدة البيانات المحلية
        Task<SyncResult> SyncOrganizationalUnitsAsync();

        // جلب من قاعدة البيانات المحلية
        Task<List<OrganizationalUnitDto>> GetLocalUnitsAsync();
        Task<List<OrganizationalUnitDto>> GetLocalChildUnitsAsync(Guid parentId);

        // الموظفين (من HR API مباشرة)
        Task<EmployeeDto?> GetEmployeeByNumberAsync(string empNumber);
        Task<List<EmployeeDto>> SearchEmployeesAsync(string searchTerm);


        void ClearCache();
    }

    /// <summary>
    /// نتيجة المزامنة
    /// </summary>
    public class SyncResult
    {
        public bool Success { get; set; }
        public int AddedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int TotalCount { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime SyncedAt { get; set; } = DateTime.Now;
    }

    public class ExternalApiService : IExternalApiService
    {
        private readonly AppDbContext _db;
        private readonly IJundClient _jundClient;
        private readonly ILogger<ExternalApiService> _logger;
        private readonly IMemoryCache _cache;

        private const string TOKEN_CACHE_KEY = "ExternalApi_AccessToken";
        private const string EMPLOYEES_CACHE_KEY = "ExternalApi_Employees";

        public ExternalApiService(
            AppDbContext db,
            IJundClient jundClient,
            ILogger<ExternalApiService> logger,
            IMemoryCache cache)
        {
            _db = db;
            _jundClient = jundClient;
            _logger = logger;
            _cache = cache;
        }

        #region Organizational Units - Sync

        /// <summary>
        /// مزامنة الوحدات التنظيمية من HR API → قاعدة البيانات المحلية
        /// يجلب كل الصفحات ويحفظ مستوى بمستوى (root → L2 → L3 → ...)
        /// </summary>
        public async Task<SyncResult> SyncOrganizationalUnitsAsync()
        {
            var result = new SyncResult();

            try
            {
                // 1. جلب كل الوحدات من HR API (كل الصفحات)
                var allApiUnits = new List<UnitTreeDto>();
                int pageNumber = 1;
                const int pageSize = 100;

                while (true)
                {
                    var page = await _jundClient.GetModUnitsAsync(pageNumber, pageSize);

                    if (page?.Items == null || page.Items.Count == 0)
                        break;

                    allApiUnits.AddRange(page.Items);

                    if (!page.HasNext)
                        break;

                    pageNumber++;
                }

                if (allApiUnits.Count == 0)
                {
                    result.ErrorMessage = "لم يتم جلب أي بيانات من API الخارجي";
                    return result;
                }

                _logger.LogInformation("Fetched {Count} units from HR API", allApiUnits.Count);

                // 2. جلب البيانات الموجودة محلياً
                var existingUnits = await _db.ExternalOrganizationalUnits.ToListAsync();

                // 3. ترتيب مستوى بمستوى (root → أبناء → أحفاد → ...)
                var allIds = allApiUnits.Select(u => u.Id).ToHashSet();
                var levels = BuildLevels(allApiUnits);

                _logger.LogInformation("Organized into {LevelCount} levels", levels.Count);

                // 4. حفظ مستوى بمستوى
                foreach (var level in levels)
                {
                    foreach (var apiUnit in level)
                    {
                        var existing = existingUnits.FirstOrDefault(u => u.Id == apiUnit.Id);

                        // التأكد إن الـ ParentId موجود فعلاً في البيانات أو في DB
                        var parentId = apiUnit.ParentId;
                        if (parentId.HasValue && !allIds.Contains(parentId.Value))
                        {
                            var parentExistsInDb = existingUnits.Any(u => u.Id == parentId.Value);
                            if (!parentExistsInDb)
                                parentId = null; // الأب مو موجود، نخليه root
                        }

                        if (existing == null)
                        {
                            _db.ExternalOrganizationalUnits.Add(new ExternalUnitEntity
                            {
                                Id = apiUnit.Id,
                                ParentId = parentId,
                                TenantId = apiUnit.TenantId,
                                Code = apiUnit.Code ?? "",
                                ArabicName = apiUnit.ArabicName ?? apiUnit.UnitName ?? "",
                                ArabicUnitName = apiUnit.ArabicName ?? apiUnit.UnitName ?? "",
                                IsActive = true,
                                LastSyncAt = DateTime.Now
                            });
                            result.AddedCount++;
                        }
                        else
                        {
                            existing.ParentId = parentId;
                            existing.TenantId = apiUnit.TenantId;
                            existing.Code = apiUnit.Code ?? existing.Code;
                            existing.ArabicName = apiUnit.ArabicName ?? apiUnit.UnitName ?? existing.ArabicName;
                            existing.ArabicUnitName = apiUnit.ArabicName ?? apiUnit.UnitName ?? existing.ArabicUnitName;
                            existing.LastSyncAt = DateTime.Now;
                            existing.IsActive = true;
                            result.UpdatedCount++;
                        }
                    }

                    // حفظ كل مستوى على حدة
                    await _db.SaveChangesAsync();
                    _logger.LogInformation("Saved level with {Count} units", level.Count);
                }

                result.Success = true;
                result.TotalCount = allApiUnits.Count;
                result.SyncedAt = DateTime.Now;

                _logger.LogInformation(
                    "Sync completed: {Added} added, {Updated} updated, {Total} total",
                    result.AddedCount, result.UpdatedCount, result.TotalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing organizational units");
                result.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            }

            return result;
        }

        /// <summary>
        /// ترتيب الوحدات حسب المستوى الهرمي
        /// المستوى 0: root (ParentId == null)
        /// المستوى 1: أبناء الـ root
        /// المستوى 2: أحفاد ... وهكذا
        /// </summary>
        private List<List<UnitTreeDto>> BuildLevels(List<UnitTreeDto> units)
        {
            var levels = new List<List<UnitTreeDto>>();
            var processed = new HashSet<Guid>();

            // المستوى 0: root units
            var currentLevel = units.Where(u => !u.ParentId.HasValue).ToList();
            if (currentLevel.Count == 0)
            {
                // لو ما فيه root — يمكن كل الـ ParentId تشير لوحدات خارج البيانات
                // نعتبرهم كلهم root
                return new List<List<UnitTreeDto>> { units };
            }

            levels.Add(currentLevel);
            foreach (var u in currentLevel)
                processed.Add(u.Id);

            // المستويات التالية
            while (processed.Count < units.Count)
            {
                var nextLevel = units
                    .Where(u => !processed.Contains(u.Id)
                             && u.ParentId.HasValue
                             && processed.Contains(u.ParentId.Value))
                    .ToList();

                if (nextLevel.Count == 0)
                {
                    // وحدات متبقية بـ ParentId مو موجود — نضيفهم كـ root
                    var remaining = units.Where(u => !processed.Contains(u.Id)).ToList();
                    if (remaining.Count > 0)
                        levels.Add(remaining);
                    break;
                }

                levels.Add(nextLevel);
                foreach (var u in nextLevel)
                    processed.Add(u.Id);
            }

            return levels;
        }

        #endregion

        #region Organizational Units - Local Database

        /// <summary>
        /// جلب كل الوحدات التنظيمية من قاعدة البيانات المحلية
        /// </summary>
        public async Task<List<OrganizationalUnitDto>> GetLocalUnitsAsync()
        {
            return await _db.ExternalOrganizationalUnits
                .Where(u => u.IsActive)
                .OrderBy(u => u.ArabicName)
                .Select(u => new OrganizationalUnitDto
                {
                    Id = u.Id,
                    ParentId = u.ParentId,
                    Name = u.ArabicName ?? u.ArabicUnitName ?? "",
                    Code = u.Code ?? ""
                })
                .ToListAsync();
        }

        /// <summary>
        /// جلب الوحدات الفرعية لوحدة معينة
        /// </summary>
        public async Task<List<OrganizationalUnitDto>> GetLocalChildUnitsAsync(Guid parentId)
        {
            return await _db.ExternalOrganizationalUnits
                .Where(u => u.IsActive && u.ParentId == parentId)
                .OrderBy(u => u.ArabicName)
                .Select(u => new OrganizationalUnitDto
                {
                    Id = u.Id,
                    ParentId = u.ParentId,
                    Name = u.ArabicName ?? u.ArabicUnitName ?? "",
                    Code = u.Code ?? ""
                })
                .ToListAsync();
        }

        #endregion

        #region Employees

        /// <summary>
        /// جلب موظف برقم الخدمة من HR API
        /// </summary>
        public async Task<EmployeeDto?> GetEmployeeByNumberAsync(string empNumber)
        {
            if (string.IsNullOrWhiteSpace(empNumber))
                return null;

            try
            {
                var employee = await _jundClient.GetEmployeeAsync(empNumber);

                if (employee == null)
                {
                    _logger.LogWarning("Employee not found: {EmpNumber}", empNumber);
                    return null;
                }

                return MapToEmployeeDto(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employee: {EmpNumber}", empNumber);
                return null;
            }
        }

        /// <summary>
        /// البحث عن موظف برقم الخدمة
        /// </summary>
        public async Task<List<EmployeeDto>> SearchEmployeesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<EmployeeDto>();

            try
            {
                var employee = await _jundClient.GetEmployeeAsync(searchTerm);

                if (employee == null)
                    return new List<EmployeeDto>();

                return new List<EmployeeDto> { MapToEmployeeDto(employee) };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Employee search failed: {SearchTerm}", searchTerm);
                return new List<EmployeeDto>();
            }
        }

        private EmployeeDto MapToEmployeeDto(Employee e) => new()
        {
            EmpNumber = e.ServiceNumber,
            Name = e.EmpNameAr,
            NameEn = e.EmpNameEn,
            Rank = e.RankArabic,
            Position = e.PositionAr,
            Unit = e.CurrentUnitAr
        };

        #endregion



        public void ClearCache()
        {
            _cache.Remove(TOKEN_CACHE_KEY);
            _cache.Remove(EMPLOYEES_CACHE_KEY);
            _logger.LogInformation("External Api cache cleared");
        }

    }
}

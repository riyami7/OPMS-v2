using Microsoft.EntityFrameworkCore;
using OperationalPlanMS.Models;
using OperationalPlanMS.Models.Entities;

namespace OperationalPlanMS.Data
{
    /// <summary>
    /// بذر البيانات الأساسية للنظام
    /// يُستدعى عند بدء التشغيل إذا كانت قاعدة البيانات فارغة
    /// يحتوي فقط على: الأدوار + مدير النظام الأعلى (SuperAdmin)
    /// </summary>
    public static class DbSeeder
    {
        /// <summary>
        /// إدراج مع تفعيل IDENTITY_INSERT — يجب فتح الاتصال يدوياً
        /// حتى يبقى SET IDENTITY_INSERT ساري المفعول على نفس الـ session
        /// </summary>
        private static async Task InsertWithIdentity<T>(AppDbContext db, string tableName, IEnumerable<T> entities) where T : class
        {
            var conn = db.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            await db.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{tableName}] ON");
            db.Set<T>().AddRange(entities);
            await db.SaveChangesAsync();
            await db.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{tableName}] OFF");

            // مسح الـ tracker حتى لا يتعارض مع الإدراجات التالية
            db.ChangeTracker.Clear();
        }

        public static async Task SeedAsync(AppDbContext db)
        {
            // لا نبذر إذا كانت البيانات موجودة مسبقاً
            if (await db.Roles.AnyAsync()) return;

            // فتح الاتصال مرة واحدة للحفاظ على session state (مطلوب لـ IDENTITY_INSERT)
            var conn = db.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            // ========== 1. الأدوار ==========
            var roles = new List<Role>
            {
                new() { Id = 1, Code = "admin",      NameAr = "مدير النظام",    NameEn = "System Admin" },
                new() { Id = 2, Code = "executive",  NameAr = "الإشراف العام",       NameEn = "Executive" },
                new() { Id = 3, Code = "supervisor", NameAr = "المدير العام",         NameEn = "Supervisor" },
                new() { Id = 4, Code = "user",       NameAr = "مدير المشروع",   NameEn = "Project Manager" },
                new() { Id = 7, Code = "stepuser",   NameAr = "منفذ الخطوة",    NameEn = "Step User" },
                new() { Id = 8, Code = "superadmin", NameAr = "مدير النظام الأعلى", NameEn = "Super Admin" },
            };
            await InsertWithIdentity(db, "Roles", roles);

            // ========== 2. مدير النظام الأعلى (SuperAdmin) ==========
            // كلمة المرور: P@ssw0rd — تم توليد الهاش باستخدام BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd", workFactor: 11);

            var users = new List<User>
            {
                new()
                {
                    Id = 1,
                    ADUsername = "admin",
                    FullNameAr = "مدير النظام الأعلى",
                    FullNameEn = "Super Administrator",
                    Email = "admin@opms.gov.om",
                    RoleId = 8,
                    PasswordHash = passwordHash,
                    ExternalUnitId = null,
                    EmployeeRank = "مدير النظام الأعلى",
                    EmployeePosition = "مدير النظام الأعلى",
                    BranchName = null,
                    IsActive = true,
                    IsStepApprover = false,
                    TenantId = null
                },
            };
            await InsertWithIdentity(db, "Users", users);
        }
    }
}

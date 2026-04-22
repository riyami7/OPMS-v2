namespace OperationalPlanMS.Models.ExternalApi
{
    /// <summary>
    /// DTO للوحدة التنظيمية - للعرض في Dropdown والتصفية
    /// </summary>
    public class OrganizationalUnitDto
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Level { get; set; }
        public List<OrganizationalUnitDto> Children { get; set; } = new();
    }

    /// <summary>
    /// DTO للموظف - للعرض في Dropdown والبحث
    /// </summary>
    public class EmployeeDto
    {
        public string EmpNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string? Rank { get; set; }
        public string? Position { get; set; }
        public string? Unit { get; set; }
        public string DisplayName => $"{Rank} {Name}".Trim();
    }
}

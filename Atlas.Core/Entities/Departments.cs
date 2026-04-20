namespace Atlas.Core.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentDepartmentId { get; set; }

        public Department? ParentDepartment { get; set; }
        public ICollection<Department> ChildDepartments { get; set; } = new List<Department>();
        public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new List<EmployeeDepartment>();
    }

    public class EmployeeDepartment
    {
        public int EmployeeId { get; set; }
        public int DepartmentId { get; set; }

        public Employee? Employee { get; set; }
        public Department? Department { get; set; }
    }
}

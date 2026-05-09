using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Atlas.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string EmployeeNumber { get; set; } = null!;
        public int PersonId { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Person? Person { get; set; }
        public EmployeeAccount? Account { get; set; }
        public ICollection<Products> Products { get; set; } = new List<Products>();
        public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new List<EmployeeDepartment>();

        [NotMapped]
        public string FullName => Person is null ? string.Empty : $"{Person.FirstName} {Person.LastName}".Trim();
    }

    public class EmployeeAccount
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLogin { get; set; }
        public int? RoleId { get; set; }

        public Employee? Employee { get; set; }
        public Role? Role { get; set; }

        [NotMapped]
        public bool CanProduct => HasPermission("PRODUCT");

        [NotMapped]
        public bool CanSale => HasPermission("SALE");

        [NotMapped]
        public bool CanEmployee => HasPermission("EMPLOYEE");

        [NotMapped]
        public bool CanInventory => HasPermission("INVENTORY");

        [NotMapped]
        public bool CanAdministration => HasPermission("ADMIN");

        [NotMapped]
        public bool CanHR => HasPermission("HR");

        private bool HasPermission(string token)
        {
            if (Role?.RolePermissions is null || string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            return Role.RolePermissions
                .Where(rp => rp.Permission is not null && !string.IsNullOrWhiteSpace(rp.Permission.PermissionKey))
                .Any(rp => rp.Permission!.PermissionKey.Contains(token, StringComparison.OrdinalIgnoreCase));
        }
    }
}

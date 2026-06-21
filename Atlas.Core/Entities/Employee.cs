using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Atlas.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string EmployeeNumber { get; set; } = null!;
        
        // Các trường dữ liệu đã được gộp trực tiếp vào Employee thay vì qua Person
        public string FullName { get; set; } = null!;
        public DateTime DoB { get; set; }
        public int AddressId { get; set; }
        public int ContactId { get; set; }
        
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        // Navigation properties mới
        public Addresses? Address { get; set; }
        public Contacts? Contact { get; set; }

        // Navigation properties hiện có
        public EmployeeAccount? Account { get; set; }
        public ICollection<Products> Products { get; set; } = new List<Products>();
        public ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new List<EmployeeDepartment>();
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

        // Logic kiểm tra phân quyền không bị ảnh hưởng bởi thay đổi Database
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
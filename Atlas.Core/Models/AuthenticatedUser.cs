namespace Atlas.Core.Models
{
    public class AuthenticatedUser
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public List<string> PermissionKeys { get; set; } = new();

        public bool CanProduct { get; set; }
        public bool CanSale { get; set; }
        public bool CanEmployee { get; set; }
        public bool CanInventory { get; set; }
        public bool CanAdministration { get; set; }
        public bool CanHR { get; set; }
    }
}

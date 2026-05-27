namespace Atlas.Core.Models
{
    public class AuthAccountSnapshot
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
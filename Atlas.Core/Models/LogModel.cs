namespace Atlas.Core.Models
{
    public class LogModel
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public string? Username { get; set; }
        public string? EmployeeNumber { get; set; }
        public string? EmployeeName { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
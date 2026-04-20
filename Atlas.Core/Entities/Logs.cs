namespace Atlas.Core.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public Employee? Employee { get; set; }
    }
}

using System;

namespace Atlas.Core.Entities
{
    public class Log
    {
        public long Id { get; set; }
        public int? EmployeeId { get; set; }
        public string Action { get; set; } = null!;
        public DateTime Timestamp { get; set; }

        // Navigation Properties
        public Employee? Employee { get; set; }
        
        // Mối quan hệ 1-1
        public LogDetail? LogDetail { get; set; } 
    }

    public class LogDetail
    {
        public long LogId { get; set; } // Vừa là PK, vừa là FK
        public string JsonChangeUrl { get; set; } = null!;

        // Navigation Property
        public Log? Log { get; set; }
    }
}
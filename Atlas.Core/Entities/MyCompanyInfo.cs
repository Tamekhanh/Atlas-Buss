using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas.Core.Entities
{
    public class MyCompanyInfo
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? TaxId { get; set; }

        [ForeignKey("LogoId")] 
        public virtual Images? LogoId { get; set; }
    }
}
using System;

namespace Atlas.Web.Areas.Party.Models
{
    public class PartyListViewModel
    {
        public int Id { get; set; }
        public string PartyType { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsVendor { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
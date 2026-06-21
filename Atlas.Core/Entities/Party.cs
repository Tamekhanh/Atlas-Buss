using System;
using System.Collections.Generic;


namespace Atlas.Core.Entities
{
    public class Party
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Phân loại đối tác: "Person" hoặc "Company"
        /// </summary>
        public string PartyType { get; set; } = null!; 
        
        public string DisplayName { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DoB { get; set; }
        public string? TaxId { get; set; }
        
        public int AddressId { get; set; }
        public int ContactId { get; set; }
        
        /// <summary>
        /// Cờ xác định vai trò của đối tác
        /// </summary>
        public bool IsCustomer { get; set; }
        public bool IsVendor { get; set; }
        
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; }

        // Navigation properties
        public Addresses? Address { get; set; }
        public Contacts? Contact { get; set; }
        
        // Navigation properties cho các giao dịch (Tùy chọn)
        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
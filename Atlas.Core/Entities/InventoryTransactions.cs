using System;

namespace Atlas.Core.Entities
{
    public class InventoryTransaction
    {
        public long Id { get; set; }
        
        // SỬA TẠI ĐÂY: Đổi ProductId thành VariantId
        public int VariantId { get; set; } 
        
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public int TransactionTypeId { get; set; } // Lookup table ID
        public string? ReferenceId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Note { get; set; }

        // MỐI QUAN HỆ (NAVIGATION PROPERTIES)
        // SỬA TẠI ĐÂY: Đổi Product thành Variant (kiểu ProductVariants)
        public ProductVariant? Variant { get; set; } 
        
        public Warehouse? Warehouse { get; set; }
        public Employee? Employee { get; set; }
        public TransactionTypes? TransactionType { get; set; }
    }

    public class TransactionTypes
    {
        public int Id { get; set; }
        
        public string TypeName { get; set; } = null!;

        // Navigation property
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
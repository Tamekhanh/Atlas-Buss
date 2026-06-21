using System;
using System.Collections.Generic;

namespace Atlas.Core.Entities
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int AddressId { get; set; }
        public int? ManagerId { get; set; }
        
        // Bổ sung các trường có trong SQL
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Addresses? Address { get; set; }
        public Employee? Manager { get; set; }
        
        public ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();
        
        // Thêm dòng này để sửa lỗi "Warehouse does not contain a definition for InventoryTransaction"
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }

    public class InventoryStock
    {
        public int WarehouseId { get; set; }
        
        // SỬA TẠI ĐÂY: Đổi ProductId thành VariantId
        public int VariantId { get; set; } 
        
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public Warehouse? Warehouse { get; set; }
        
        // SỬA TẠI ĐÂY: Đổi Products thành ProductVariants (hoặc class tương ứng của bạn)
        public ProductVariant? Variant { get; set; } 
    }
}
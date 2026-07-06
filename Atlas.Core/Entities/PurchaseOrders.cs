using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas.Core.Entities
{
    [Table("PurchaseOrders")] // Khớp chính xác tên bảng SQL
    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }
        
        public string PONumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        public int VendorId { get; set; }
        [ForeignKey("VendorId")]
        public virtual Party? Vendor { get; set; }

        // GIẢI QUYẾT LỖI: Invalid column name 'PurchaseOrderStatusId'
        public int OrderStatusId { get; set; } 
        
        [ForeignKey("OrderStatusId")] // BẮT BUỘC: Ép EF Core dùng đúng cột OrderStatusId
        public virtual PurchaseOrderStatuses? OrderStatus { get; set; }

        public int CurrencyId { get; set; } = 1;
        [ForeignKey("CurrencyId")]
        public virtual Currencies? Currency { get; set; }

        public decimal ExchangeRate { get; set; } = 1.0m;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }

    [Table("PurchaseOrderStatuses")] // Khớp tên bảng SQL
    public class PurchaseOrderStatuses
    {
        [Key]
        public int Id { get; set; }
        public string StatusName { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }

    [Table("PurchaseOrderDetails")] // Khớp tên bảng SQL
    public class PurchaseOrderDetail
    {
        [Key]
        public int Id { get; set; }
        
        public int POId { get; set; }
        [ForeignKey("POId")]
        public virtual PurchaseOrder? PurchaseOrder { get; set; }

        public int VariantId { get; set; }
        [ForeignKey("VariantId")]
        public virtual ProductVariant? Variant { get; set; }

        public int WarehouseId { get; set; }
        [ForeignKey("WarehouseId")]
        public virtual Warehouse? Warehouse { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } 
        public decimal Discount { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0;

        // GIẢI QUYẾT LỖI khi Save: Computed Columns
        // Trong SQL bạn dùng 'AS', nên trong C# KHÔNG ĐƯỢC dùng '=>'
        // Phải dùng property bình thường và đánh dấu là Computed.
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        public decimal SubTotal { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        public decimal LineTotal { get; set; }

        public string? BillUrl { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Atlas.Core.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int EmployeeId { get; set; }

        // CHUẨN HÓA: Thay thế VendorCompanyId và VendorPersonId bằng VendorId duy nhất
        public int VendorId { get; set; }

        public int OrderStatusId { get; set; } = 1;
        public int CurrencyId { get; set; } = 1; // Thêm CurrencyId cho đa tiền tệ
        public decimal ExchangeRate { get; set; } = 1.0m;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public Employee? Employee { get; set; }

        // CHUẨN HÓA: Trỏ về Party thay vì VendorCompany/VendorPerson
        public Party? Vendor { get; set; }

        public PurchaseOrderStatuses? OrderStatus { get; set; }
        public Currencies? Currency { get; set; }
        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }

    public class PurchaseOrderStatuses
    {
        public int Id { get; set; }
        public string StatusName { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }

    public class PurchaseOrderDetail
    {
        public int Id { get; set; }
        public int POId { get; set; }

        // CHUẨN HÓA: Mua hàng theo Biến thể thay vì Sản phẩm cha
        public int VariantId { get; set; }

        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Kiểu decimal
        public decimal Discount { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0; // Thay TaxRate bằng TaxAmount để tính toán chính xác nhiều thuế

        // Các computed properties
        public decimal SubTotal => (Quantity * UnitPrice) - Discount;
        public decimal LineTotal => ((Quantity * UnitPrice) - Discount) + TaxAmount;

        // Navigation Properties
        public PurchaseOrder? PurchaseOrder { get; set; }

        // CHUẨN HÓA: Trỏ về ProductVariant
        public ProductVariant? Variant { get; set; }

        public Warehouse? Warehouse { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Atlas.Core.Entities
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int EmployeeId { get; set; }
        
        // CHUẨN HÓA: Thay thế CustomerCompanyId và CustomerPersonId bằng CustomerId duy nhất
        public int CustomerId { get; set; }
        
        public int OrderStatusId { get; set; } = 1;
        public int CurrencyId { get; set; } = 1; // Hỗ trợ Đa tiền tệ
        public decimal ExchangeRate { get; set; } = 1.0m;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public Employee? Employee { get; set; }
        
        // CHUẨN HÓA: Trỏ về Party thay vì CustomerCompany/CustomerPerson
        public Party? Customer { get; set; }
        
        // Lưu ý: Đảm bảo class này tên là SalesOrderStatuses (số nhiều) nếu bạn đã tạo theo mẫu trước đó
        public SalesOrderStatuses? OrderStatus { get; set; }
        public Currencies? Currency { get; set; }
        public ICollection<SalesOrderDetail> SalesOrderDetails { get; set; } = new List<SalesOrderDetail>();
    }

    public class SalesOrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        
        // CHUẨN HÓA: Xuất bán theo Biến thể (SKU) thay vì Sản phẩm cha
        public int VariantId { get; set; }
        
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; } = 0;
        
        // CHUẨN HÓA: Dùng TaxAmount thay cho TaxRate để tránh sai số khi áp dụng nhiều loại thuế
        public decimal TaxAmount { get; set; } = 0;

        // Computed Properties
        public decimal SubTotal => (Quantity * UnitPrice) - Discount;
        public decimal LineTotal => ((Quantity * UnitPrice) - Discount) + TaxAmount;

        // Navigation Properties
        public SalesOrder? SalesOrder { get; set; }
        
        // CHUẨN HÓA: Trỏ về ProductVariant
        public ProductVariant? Variant { get; set; }
        
        public Warehouse? Warehouse { get; set; }
    }

    public class SalesOrderStatuses
    {
        public int Id { get; set; }
        public string StatusName { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    }
}
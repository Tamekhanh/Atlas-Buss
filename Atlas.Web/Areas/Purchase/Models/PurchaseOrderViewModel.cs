using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Web.Areas.PurchaseOrder.Models 
{
    public class PurchaseOrderVendorLookupVM
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }

    public class PurchaseOrderWarehouseLookupVM
    {
        public int Id { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
    }

    public class PurchaseOrderProductLookupVM
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public List<PurchaseOrderVariantLookupVM> Variants { get; set; } = new();
    }

    public class PurchaseOrderVariantLookupVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal? VariantPrice { get; set; }
    }

    public class PurchaseOrderVM
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public int OrderStatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class PurchaseOrderCreateVM
    {
        public string PONumber { get; set; } = string.Empty;
        [Required]
        public int VendorId { get; set; }
        public int OrderStatusId { get; set; } = 1;
        public int CurrencyId { get; set; } = 1;
        public decimal ExchangeRate { get; set; } = 1.0m;
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public List<PurchaseOrderVendorLookupVM> Vendors { get; set; } = new List<PurchaseOrderVendorLookupVM>();
        public List<PurchaseOrderProductLookupVM> Products { get; set; } = new List<PurchaseOrderProductLookupVM>();
        public List<PurchaseOrderWarehouseLookupVM> Warehouses { get; set; } = new List<PurchaseOrderWarehouseLookupVM>();
        public List<PurchaseOrderDetailVM> OrderDetails { get; set; } = new List<PurchaseOrderDetailVM>();
    }

    public class PurchaseOrderDetailVM
    {
        public int ProductId { get; set; }
        public int VariantId { get; set; }
        public string VariantSKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
    }

    // View-model cho trang Detail (xem chi tiết đơn mua hàng)
    public class PurchaseOrderDetailPageVM
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int OrderStatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal ExchangeRate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal GrandTotal { get; set; }
        public List<PurchaseOrderDetailLineVM> Lines { get; set; } = new();
        public List<PurchaseOrderBillVM> Bills { get; set; } = new();
    }

    public class PurchaseOrderBillVM
    {
        public int Id { get; set; }
        public string BillUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class PurchaseOrderDetailLineVM
    {
        public int VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string VariantSKU { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
    }
}
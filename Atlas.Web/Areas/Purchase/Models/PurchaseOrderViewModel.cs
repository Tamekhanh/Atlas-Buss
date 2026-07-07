using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Web.Areas.PurchaseOrder.Models 
{
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
        [Required]
        public string PONumber { get; set; } = string.Empty;
        [Required]
        public int VendorId { get; set; }
        public int OrderStatusId { get; set; } = 1;
        public int CurrencyId { get; set; } = 1;
        public decimal ExchangeRate { get; set; } = 1.0m;
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public List<PurchaseOrderProductLookupVM> Products { get; set; } = new List<PurchaseOrderProductLookupVM>();
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
}
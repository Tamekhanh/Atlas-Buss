using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Web.Areas.SaleOrder.Models
{
	public class SaleOrderCustomerLookupVM
	{
		public int Id { get; set; }
		public string DisplayName { get; set; } = string.Empty;
	}

	public class SaleOrderProductLookupVM
	{
		public int Id { get; set; }
		public string ProductName { get; set; } = string.Empty;
		public List<SaleOrderVariantLookupVM> Variants { get; set; } = new();
	}

	public class SaleOrderVariantLookupVM
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public string SKU { get; set; } = string.Empty;
		public decimal? VariantPrice { get; set; }
		public string AttributeText { get; set; } = string.Empty;
	}

	public class SaleOrderWarehouseLookupVM
	{
		public int Id { get; set; }
		public string WarehouseName { get; set; } = string.Empty;
	}

	public class SaleOrderVM
	{
		public int Id { get; set; }
		public string OrderNumber { get; set; } = string.Empty;
		public DateTime OrderDate { get; set; } = DateTime.Now;
		public int CustomerId { get; set; }
		public string CustomerName { get; set; } = string.Empty;
		public int OrderStatusId { get; set; }
		public string StatusName { get; set; } = string.Empty;
		public decimal TotalAmount { get; set; }
	}

	public class SaleOrderCreateVM
	{
		public string OrderNumber { get; set; } = string.Empty;

		[Required]
		public int CustomerId { get; set; }

		public int OrderStatusId { get; set; } = 1;
		public int CurrencyId { get; set; } = 1;
		public decimal ExchangeRate { get; set; } = 1.0m;
		public DateTime OrderDate { get; set; } = DateTime.Now;

		public List<SaleOrderCustomerLookupVM> Customers { get; set; } = new();
		public List<SaleOrderProductLookupVM> Products { get; set; } = new();
		public List<SaleOrderWarehouseLookupVM> Warehouses { get; set; } = new();
		public List<SaleOrderDetailVM> OrderDetails { get; set; } = new();
	}

	public class SaleOrderDetailVM
	{
		public int VariantId { get; set; }
		public int WarehouseId { get; set; }
		public int Quantity { get; set; } = 1;
		public decimal UnitPrice { get; set; }
		public decimal Discount { get; set; }
		public decimal TaxAmount { get; set; }
	}

	// View-model cho trang Detail (xem chi tiết đơn bán hàng)
	public class SaleOrderDetailPageVM
	{
		public int Id { get; set; }
		public string OrderNumber { get; set; } = string.Empty;
		public DateTime OrderDate { get; set; }
		public int CustomerId { get; set; }
		public string CustomerName { get; set; } = string.Empty;
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
		public List<SaleOrderDetailLineVM> Lines { get; set; } = new();
		public List<SaleOrderBillVM> Bills { get; set; } = new();
	}

	public class SaleOrderBillVM
	{
		public int Id { get; set; }
		public string BillUrl { get; set; } = string.Empty;
		public string BillSource { get; set; } = "Uploaded";
		public DateTime CreatedAt { get; set; }
	}

	// View-model cho danh mục mẫu in bill (Bill Template).
	public class BillTemplateVM
	{
		public int Id { get; set; }
		public string TemplateName { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string PageSize { get; set; } = "A4";
		public string Orientation { get; set; } = "Portrait";
		public bool ShowLogo { get; set; } = true;
		public bool ShowTaxBreakdown { get; set; } = true;
		public bool ShowSignatureLine { get; set; } = true;
		public bool ShowGrandTotalBox { get; set; } = true;
		public bool ShowCustomerInfo { get; set; } = true;
		public bool ShowWarehouseColumn { get; set; } = true;
		public string? HeaderNote { get; set; }
		public string? FooterNote { get; set; }
		public bool IsDefault { get; set; }
	}

	public class BillTemplateEditVM
	{
		public int Id { get; set; }
		[Required, StringLength(100)]
		public string TemplateName { get; set; } = string.Empty;
		[StringLength(255)]
		public string? Description { get; set; }
		public string PageSize { get; set; } = "A4";
		public string Orientation { get; set; } = "Portrait";
		public bool ShowLogo { get; set; } = true;
		public bool ShowTaxBreakdown { get; set; } = true;
		public bool ShowSignatureLine { get; set; } = true;
		public bool ShowGrandTotalBox { get; set; } = true;
		public bool ShowCustomerInfo { get; set; } = true;
		public bool ShowWarehouseColumn { get; set; } = true;
		[StringLength(500)]
		public string? HeaderNote { get; set; }
		[StringLength(500)]
		public string? FooterNote { get; set; }
		public bool IsDefault { get; set; }
	}

	public class SaleOrderDetailLineVM
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
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
		// Tiêu đề bill tùy chỉnh
		public string? BillTitle { get; set; }
		public string? BillSubtitle { get; set; }
		// Cột dòng hàng
		public bool ShowSkuColumn { get; set; } = true;
		public bool ShowDescriptionColumn { get; set; } = false;
		// Trường bổ sung
		public bool ShowAmountInWords { get; set; } = false;
		public bool ShowCurrencyCode { get; set; } = true;
		public bool ShowExchangeRate { get; set; } = true;
		public bool ShowPageNumbers { get; set; } = true;
		// Màu nhấn & kiểu dáng
		public string? AccentColorHex { get; set; }
		public int LogoMaxHeight { get; set; } = 50;
		public int PageMargin { get; set; } = 40;
		public string GrandTotalBoxStyle { get; set; } = "Box";
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
		// Tiêu đề bill tùy chỉnh
		[StringLength(100), Display(Name = "Bill Title")]
		public string? BillTitle { get; set; }
		[StringLength(200), Display(Name = "Bill Subtitle")]
		public string? BillSubtitle { get; set; }
		// Cột dòng hàng
		public bool ShowSkuColumn { get; set; } = true;
		public bool ShowDescriptionColumn { get; set; } = false;
		// Trường bổ sung
		public bool ShowAmountInWords { get; set; } = false;
		public bool ShowCurrencyCode { get; set; } = true;
		public bool ShowExchangeRate { get; set; } = true;
		public bool ShowPageNumbers { get; set; } = true;
		// Màu nhấn & kiểu dáng
		[StringLength(20), Display(Name = "Accent Color")]
		public string? AccentColorHex { get; set; }
		[Range(0, 400), Display(Name = "Logo Max Height (px)")]
		public int LogoMaxHeight { get; set; } = 50;
		[Range(0, 120), Display(Name = "Page Margin (px)")]
		public int PageMargin { get; set; } = 40;
		[StringLength(20), Display(Name = "Grand Total Box Style")]
		public string GrandTotalBoxStyle { get; set; } = "Box";
		[StringLength(4000)]
		public string? HeaderNote { get; set; }
		[StringLength(4000)]
		public string? FooterNote { get; set; }
		public bool IsDefault { get; set; }
	}

	// View-model cho nút Preview trên form tạo/sửa Bill Template.
	// Chỉ mang OrderId được chọn + các tùy chọn in (mirror BillTemplateEditVM) để POST lên endpoint Preview.
	public class BillTemplatePreviewVM
	{
		[Required]
		public int OrderId { get; set; }

		// Page / orientation
		public string PageSize { get; set; } = "A4";
		public string Orientation { get; set; } = "Portrait";

		// Tùy chọn in (giống BillTemplateOptions)
		public bool ShowLogo { get; set; } = true;
		public bool ShowTaxBreakdown { get; set; } = true;
		public bool ShowSignatureLine { get; set; } = true;
		public bool ShowGrandTotalBox { get; set; } = true;
		public bool ShowCustomerInfo { get; set; } = true;
		public bool ShowWarehouseColumn { get; set; } = true;
		public string? BillTitle { get; set; }
		public string? BillSubtitle { get; set; }
		public bool ShowSkuColumn { get; set; } = true;
		public bool ShowDescriptionColumn { get; set; } = false;
		public bool ShowAmountInWords { get; set; } = false;
		public bool ShowCurrencyCode { get; set; } = true;
		public bool ShowExchangeRate { get; set; } = true;
		public bool ShowPageNumbers { get; set; } = true;
		public string? AccentColorHex { get; set; }
		public int LogoMaxHeight { get; set; } = 50;
		public int PageMargin { get; set; } = 40;
		public string GrandTotalBoxStyle { get; set; } = "Box";
		public string? HeaderNote { get; set; }
		public string? FooterNote { get; set; }
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
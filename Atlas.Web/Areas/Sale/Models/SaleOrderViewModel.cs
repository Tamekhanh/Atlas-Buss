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
		[Required]
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
}
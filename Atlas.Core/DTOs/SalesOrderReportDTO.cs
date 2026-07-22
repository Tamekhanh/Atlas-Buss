using System;
using System.Collections.Generic;

namespace Atlas.Core.DTOs
{
    /// <summary>
    /// Các tùy chọn in được lưu trong BillTemplates.OptionsJson (dạng JSON).
    /// </summary>
    public class BillTemplateOptions
    {
        public bool ShowLogo { get; set; } = true;
        public bool ShowTaxBreakdown { get; set; } = true;
        public bool ShowSignatureLine { get; set; } = true;
        public bool ShowGrandTotalBox { get; set; } = true;
        public bool ShowCustomerInfo { get; set; } = true;
        public bool ShowWarehouseColumn { get; set; } = true;
    }

    /// <summary>
    /// Dữ liệu đã được tổng hợp sẵn để đưa vào tài liệu PDF in bill Sales Order.
    /// Tách biệt khỏi entity để QuestPDF không phụ thuộc trực tiếp EF navigation.
    /// </summary>
    public class SalesOrderReportData
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string StatusName { get; set; } = string.Empty;

        // Company (header)
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyAddress { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyTaxId { get; set; }
        public byte[]? LogoBytes { get; set; }

        // Customer
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerAddress { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }

        // Employee
        public string EmployeeName { get; set; } = string.Empty;

        // Currency
        public string CurrencyCode { get; set; } = string.Empty;
        public decimal ExchangeRate { get; set; } = 1.0m;

        // Totals
        public decimal SubTotal { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal GrandTotal { get; set; }

        // Template appearance
        public string PageSize { get; set; } = "A4";
        public string Orientation { get; set; } = "Portrait";
        public BillTemplateOptions Options { get; set; } = new();
        public string? HeaderNote { get; set; }
        public string? FooterNote { get; set; }

        public List<SalesOrderReportLine> Lines { get; set; } = new();
    }

    public class SalesOrderReportLine
    {
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
    }
}

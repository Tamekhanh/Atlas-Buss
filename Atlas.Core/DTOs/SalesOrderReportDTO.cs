using System;
using System.Collections.Generic;

namespace Atlas.Core.DTOs
{
    /// <summary>
    /// Các tùy chọn in được lưu trong BillTemplates.OptionsJson (dạng JSON).
    /// Mọi tùy chọn mới đều lưu ở đây (không thêm cột DB) để giữ cho schema gọn.
    /// </summary>
    public class BillTemplateOptions
    {
        // === Bật/tắt các khối thông tin ===
        public bool ShowLogo { get; set; } = true;
        public bool ShowTaxBreakdown { get; set; } = true;
        public bool ShowSignatureLine { get; set; } = true;
        public bool ShowGrandTotalBox { get; set; } = true;
        public bool ShowCustomerInfo { get; set; } = true;
        public bool ShowWarehouseColumn { get; set; } = true;

        // === Tiêu đề bill tùy chỉnh ===
        // Rỗng/null => dùng mặc định "SALES ORDER".
        public string? BillTitle { get; set; }
        public string? BillSubtitle { get; set; }

        // === Cột dòng hàng (line items) ===
        // SKU hiện đang in bắt buộc; cho phép ẩn đi.
        public bool ShowSkuColumn { get; set; } = true;
        // Cột mô tả sản phẩm (lấy từ Product.Description / Product.ProductDescription).
        public bool ShowDescriptionColumn { get; set; } = false;

        // === Trường bổ sung ===
        // In tổng tiền bằng chữ (tiếng Anh).
        public bool ShowAmountInWords { get; set; } = false;
        public bool ShowCurrencyCode { get; set; } = true;
        public bool ShowExchangeRate { get; set; } = true;
        public bool ShowPageNumbers { get; set; } = true;

        // === Màu nhấn & kiểu dáng ===
        // Mã HEX (vd "#1F77B4"); rỗng/null => dùng màu mặc định (Grey).
        public string? AccentColorHex { get; set; }
        // Chiều cao tối đa logo (px). 0 => dùng mặc định 50.
        public int LogoMaxHeight { get; set; } = 50;
        // Lề trang (px). 0 => dùng mặc định 40.
        public int PageMargin { get; set; } = 40;
        // Kiểu viền hộp grand total: "Box" | "Line" | "None". Mặc định "Box".
        public string GrandTotalBoxStyle { get; set; } = "Box";
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
        public string? Description { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal LineTotal { get; set; }
    }
}

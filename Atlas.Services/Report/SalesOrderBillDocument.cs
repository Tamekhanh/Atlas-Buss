using Atlas.Core.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Globalization;
using System.Linq;

namespace Atlas.Services.Report
{
    /// <summary>
    /// Tài liệu PDF in bill Sales Order (QuestPDF).
    /// Nhận một SalesOrderReportData đã được tổng hợp sẵn.
    /// </summary>
    public class SalesOrderBillDocument : IDocument
    {
        private readonly SalesOrderReportData _data;

        public SalesOrderBillDocument(SalesOrderReportData data)
        {
            _data = data;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            var opts = _data.Options;
            // Lề trang: 0 => mặc định 40. Giới hạn trong khoảng hợp lý để tránh layout vỡ.
            var margin = opts.PageMargin > 0 ? opts.PageMargin : 40;

            container
                .Page(page =>
                {
                    page.Size(MapPageSize(_data.PageSize, _data.Orientation));
                    page.Margin(margin);
                    page.DefaultTextStyle(ts => ts.FontSize(10));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        // Quy đổi mã HEX (vd "#1F77B4") sang QuestPDF Color.
        // Trả về grey làm fallback nếu mã không hợp lệ.
        private static Color ParseAccentColor(string? hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return Colors.Grey.Darken1;
            try { return Color.FromHex(hex.Trim()); }
            catch { return Colors.Grey.Darken1; }
        }

        // ===== Header: logo + thông tin công ty + tiêu đề bill =====
        void ComposeHeader(IContainer container)
        {
            var opts = _data.Options;
            var accent = ParseAccentColor(opts.AccentColorHex);
            var logoHeight = opts.LogoMaxHeight > 0 ? opts.LogoMaxHeight : 50;
            // Tiêu đề bill tùy chỉnh; rỗng => mặc định "SALES ORDER".
            var billTitle = string.IsNullOrWhiteSpace(opts.BillTitle) ? "SALES ORDER" : opts.BillTitle.Trim();

            container.PaddingBottom(10).Column(column =>
            {
                column.Item().Row(row =>
                {
                    // Cột trái: logo + thông tin công ty
                    row.RelativeItem().Column(col =>
                    {
                        if (opts.ShowLogo && _data.LogoBytes is { Length: > 0 })
                        {
                            col.Item().Height(logoHeight).MaxWidth(160).Image(_data.LogoBytes);
                        }

                        col.Item().Text(_data.CompanyName).FontSize(16).Bold().FontColor(accent);
                        col.Item().Text(_data.CompanyAddress ?? string.Empty).FontSize(9).FontColor(Colors.Grey.Darken1);
                        var contactLine = JoinNonEmpty(" | ",
                            _data.CompanyPhone, _data.CompanyEmail,
                            string.IsNullOrWhiteSpace(_data.CompanyTaxId) ? null : $"Tax ID: {_data.CompanyTaxId}");
                        if (!string.IsNullOrWhiteSpace(contactLine))
                        {
                            col.Item().Text(contactLine).FontSize(9).FontColor(Colors.Grey.Darken1);
                        }
                    });

                    // Cột phải: tiêu đề bill + phụ đề + số + ngày
                    row.ConstantItem(220).AlignRight().Column(col =>
                    {
                        col.Item().Text(billTitle).FontSize(20).Bold().FontColor(accent);
                        if (!string.IsNullOrWhiteSpace(opts.BillSubtitle))
                        {
                            col.Item().Text(opts.BillSubtitle.Trim()).FontSize(9).FontColor(Colors.Grey.Darken1);
                        }
                        col.Item().Text(_data.OrderNumber).FontSize(12).Bold();
                        col.Item().Text($"Date: {_data.OrderDate:dd/MM/yyyy}").FontSize(9);
                        if (!string.IsNullOrWhiteSpace(_data.StatusName))
                        {
                            col.Item().Text($"Status: {_data.StatusName}").FontSize(9).FontColor(Colors.Grey.Darken1);
                        }
                    });
                });

                // Ghi chú đầu bill (HeaderNote)
                if (!string.IsNullOrWhiteSpace(_data.HeaderNote))
                {
                    column.Spacing(4);
                    column.Item().PaddingTop(6).BorderBottom(1).BorderColor(accent).Text(_data.HeaderNote).FontSize(9).Italic();
                }
            });
        }

        // ===== Content: thông tin khách + bảng line items + totals =====
        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(10).Column(column =>
            {
                if (_data.Options.ShowCustomerInfo)
                {
                    column.Spacing(4);
                    column.Item().Text("Bill To").FontSize(11).Bold();
                    column.Item().Text(_data.CustomerName).Bold();
                    if (!string.IsNullOrWhiteSpace(_data.CustomerAddress))
                        column.Item().Text(_data.CustomerAddress).FontSize(9).FontColor(Colors.Grey.Darken1);
                    var contact = JoinNonEmpty(" | ", _data.CustomerPhone, _data.CustomerEmail);
                    if (!string.IsNullOrWhiteSpace(contact))
                        column.Item().Text(contact).FontSize(9).FontColor(Colors.Grey.Darken1);

                    column.Item().Text($"Salesperson: {_data.EmployeeName}").FontSize(9).FontColor(Colors.Grey.Darken1);
                }

                column.Spacing(8);

                // Bảng line items
                column.Item().Element(ComposeLineItemsTable);

                column.Spacing(8);

                // Totals
                column.Item().Element(ComposeTotals);
            });
        }

        void ComposeLineItemsTable(IContainer container)
        {
            var opts = _data.Options;
            var accent = ParseAccentColor(opts.AccentColorHex);

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);   // Product
                    if (opts.ShowSkuColumn)
                        columns.RelativeColumn(2); // SKU
                    if (opts.ShowDescriptionColumn)
                        columns.RelativeColumn(3); // Description
                    if (opts.ShowWarehouseColumn)
                        columns.RelativeColumn(2); // Warehouse
                    columns.ConstantColumn(60);   // Qty
                    columns.ConstantColumn(80);   // Unit Price
                    columns.ConstantColumn(70);   // Discount
                    columns.ConstantColumn(70);   // Tax
                    columns.ConstantColumn(80);    // Line Total
                });

                table.Header(header =>
                {
                    header.Cell().Element(h => HeaderCellStyle(h, accent)).Text("Product").Bold().FontColor(Colors.White);
                    if (opts.ShowSkuColumn)
                        header.Cell().Element(h => HeaderCellStyle(h, accent)).Text("SKU").Bold().FontColor(Colors.White);
                    if (opts.ShowDescriptionColumn)
                        header.Cell().Element(h => HeaderCellStyle(h, accent)).Text("Description").Bold().FontColor(Colors.White);
                    if (opts.ShowWarehouseColumn)
                        header.Cell().Element(h => HeaderCellStyle(h, accent)).Text("Warehouse").Bold().FontColor(Colors.White);
                    header.Cell().Element(h => HeaderCellStyle(h, accent)).AlignRight().Text("Qty").Bold().FontColor(Colors.White);
                    header.Cell().Element(h => HeaderCellStyle(h, accent)).AlignRight().Text("Unit Price").Bold().FontColor(Colors.White);
                    header.Cell().Element(h => HeaderCellStyle(h, accent)).AlignRight().Text("Discount").Bold().FontColor(Colors.White);
                    header.Cell().Element(h => HeaderCellStyle(h, accent)).AlignRight().Text("Tax").Bold().FontColor(Colors.White);
                    header.Cell().Element(h => HeaderCellStyle(h, accent)).AlignRight().Text("Line Total").Bold().FontColor(Colors.White);
                });

                foreach (var line in _data.Lines)
                {
                    table.Cell().Element(CellStyle).Text(line.ProductName);
                    if (opts.ShowSkuColumn)
                        table.Cell().Element(CellStyle).Text(line.Sku);
                    if (opts.ShowDescriptionColumn)
                        table.Cell().Element(CellStyle).Text(line.Description ?? string.Empty);
                    if (opts.ShowWarehouseColumn)
                        table.Cell().Element(CellStyle).Text(line.WarehouseName);
                    table.Cell().Element(CellStyle).AlignRight().Text(line.Quantity.ToString("N0"));
                    table.Cell().Element(CellStyle).AlignRight().Text(line.UnitPrice.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignRight().Text(line.Discount.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignRight().Text(line.TaxAmount.ToString("N2"));
                    table.Cell().Element(CellStyle).AlignRight().Text(line.LineTotal.ToString("N2")).Bold();
                }
            });
        }

        void ComposeTotals(IContainer container)
        {
            var opts = _data.Options;
            var accent = ParseAccentColor(opts.AccentColorHex);
            // Nhãn grand total có thể ẩn mã tiền tệ.
            var grandTotalLabel = opts.ShowCurrencyCode && !string.IsNullOrWhiteSpace(_data.CurrencyCode)
                ? $"Grand Total ({_data.CurrencyCode})"
                : "Grand Total";

            container.AlignRight().Column(column =>
            {
                column.Spacing(2);
                column.Item().Row(row =>
                {
                    row.ConstantItem(160).Text("Subtotal").FontColor(Colors.Grey.Darken1);
                    row.ConstantItem(100).AlignRight().Text(Format(_data.SubTotal));
                });

                column.Item().Row(row =>
                {
                    row.ConstantItem(160).Text("Total Discount").FontColor(Colors.Grey.Darken1);
                    row.ConstantItem(100).AlignRight().Text(Format(_data.TotalDiscount));
                });

                if (opts.ShowTaxBreakdown)
                {
                    column.Item().Row(row =>
                    {
                        row.ConstantItem(160).Text("Total Tax").FontColor(Colors.Grey.Darken1);
                        row.ConstantItem(100).AlignRight().Text(Format(_data.TotalTax));
                    });
                }

                // Kiểu hộp grand total: Box (nền) | Line (gạch ngang trên) | None (chỉ text).
                var boxStyle = (opts.GrandTotalBoxStyle ?? "Box").Trim();
                if (opts.ShowGrandTotalBox && string.Equals(boxStyle, "Box", StringComparison.OrdinalIgnoreCase))
                {
                    column.Item().PaddingTop(6);
                    column.Item().Background(Colors.Grey.Lighten3).Padding(8).Row(row =>
                    {
                        row.ConstantItem(160).Text(grandTotalLabel).Bold();
                        row.ConstantItem(100).AlignRight().Text(Format(_data.GrandTotal)).Bold().FontSize(12);
                    });
                }
                else if (opts.ShowGrandTotalBox && string.Equals(boxStyle, "Line", StringComparison.OrdinalIgnoreCase))
                {
                    column.Item().PaddingTop(6);
                    column.Item().BorderTop(1).BorderColor(accent).PaddingTop(6).Row(row =>
                    {
                        row.ConstantItem(160).Text(grandTotalLabel).Bold();
                        row.ConstantItem(100).AlignRight().Text(Format(_data.GrandTotal)).Bold().FontSize(12);
                    });
                }
                else
                {
                    // "None" hoặc ShowGrandTotalBox = false: chỉ hiển thị text.
                    column.Item().PaddingTop(4);
                    column.Item().Row(row =>
                    {
                        row.ConstantItem(160).Text(grandTotalLabel).Bold();
                        row.ConstantItem(100).AlignRight().Text(Format(_data.GrandTotal)).Bold();
                    });
                }

                // In tổng tiền bằng chữ (tiếng Anh) nếu bật.
                if (opts.ShowAmountInWords)
                {
                    var words = NumberToEnglish(_data.GrandTotal);
                    if (!string.IsNullOrWhiteSpace(words))
                    {
                        column.Item().PaddingTop(2).AlignLeft().Text($"Amount in words: {words}").FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                    }
                }

                // Tỉ giá: chỉ in khi bật và có giá trị khác 1.
                if (opts.ShowExchangeRate && _data.ExchangeRate != 1.0m && !string.IsNullOrWhiteSpace(_data.CurrencyCode))
                {
                    column.Item().Text($"Exchange rate: {_data.ExchangeRate.ToString("N4", CultureInfo.InvariantCulture)}").FontSize(8).FontColor(Colors.Grey.Darken1);
                }
            });
        }

        // ===== Footer: ghi chú cuối + dòng chữ ký =====
        void ComposeFooter(IContainer container)
        {
            container.PaddingTop(12).Column(column =>
            {
                if (_data.Options.ShowSignatureLine)
                {
                    column.Spacing(40);
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().PaddingBottom(40);
                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Darken2);
                            col.Item().Text("Customer Signature");
                        });
                        row.ConstantItem(40);
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().PaddingBottom(40);
                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Darken2);
                            col.Item().Text("For " + (_data.CompanyName ?? "Company"));
                        });
                    });
                }

                if (!string.IsNullOrWhiteSpace(_data.FooterNote))
                {
                    column.Item().AlignCenter().Text(_data.FooterNote).FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                }

                if (_data.Options.ShowPageNumbers)
                {
                    column.Item().AlignCenter().Text(t =>
                    {
                        t.CurrentPageNumber();
                        t.Span(" / ");
                        t.TotalPages();
                    });
                }
            });
        }

        // ===== Helpers =====
        static IContainer CellStyle(IContainer container)
            => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4);

        // Ô tiêu đề cột: tô nền bằng màu nhấn, chữ trắng.
        static IContainer HeaderCellStyle(IContainer container, Color accent)
            => container.Background(accent).PaddingVertical(4).PaddingHorizontal(4);

        static string Format(decimal value) => value.ToString("N2");

        static string? JoinNonEmpty(string separator, params string?[] parts)
            => string.Join(separator, parts.Where(p => !string.IsNullOrWhiteSpace(p)));

        // Đổi số tiền sang chữ tiếng Anh (vd "One Thousand Two Hundred Thirty Four and 56/100").
        // Hỗ trợ phần nguyên tối đa hàng tỷ; phần thập phân (2 chữ số) => cents.
        static string NumberToEnglish(decimal value)
        {
            if (value == 0m) return "Zero";

            var rounded = Math.Round(value, 2);
            var intPart = (long)Math.Truncate(rounded);
            var frac = (int)Math.Round((rounded - intPart) * 100m);
            // Xử lệ tràn làm tròn (vd 99.999 -> intPart tăng 1, frac = 0).
            if (frac >= 100) { intPart += 1; frac -= 100; }

            var words = ConvertLongToWords(intPart);
            if (frac > 0)
                words += $" and {frac}/100";
            return words;
        }

        static string ConvertLongToWords(long n)
        {
            if (n == 0) return "Zero";
            if (n < 0) return "Minus " + ConvertLongToWords(-n);

            var parts = new System.Collections.Generic.List<string>();
            foreach (var (divisor, name) in new (long, string)[]
            {
                (1_000_000_000_000L, "Trillion"),
                (1_000_000_000L, "Billion"),
                (1_000_000L, "Million"),
                (1_000L, "Thousand"),
            })
            {
                if (n >= divisor)
                {
                    var count = (int)(n / divisor);
                    parts.Add($"{ThreeDigitsToWords(count)} {name}");
                    n %= divisor;
                }
            }
            if (n > 0)
                parts.Add(ThreeDigitsToWords((int)n));

            return string.Join(" ", parts).Trim();
        }

        // Đổi số 0..999 sang chữ (vd 123 => "One Hundred Twenty-Three").
        static string ThreeDigitsToWords(int n)
        {
            if (n == 0) return "";
            if (n < 0) return "Minus " + ThreeDigitsToWords(-n);

            var ones = new[]
            {
                "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
                "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
                "Seventeen", "Eighteen", "Nineteen"
            };
            var tens = new[] { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            var parts = new System.Collections.Generic.List<string>();
            if (n >= 100)
            {
                parts.Add($"{ones[n / 100]} Hundred");
                n %= 100;
            }
            if (n >= 20)
            {
                var tensPart = tens[n / 10];
                var onesPart = n % 10;
                parts.Add(onesPart == 0 ? tensPart : $"{tensPart}-{ones[onesPart]}");
            }
            else if (n > 0)
            {
                parts.Add(ones[n]);
            }
            return string.Join(" ", parts);
        }

        static PageSize MapPageSize(string pageSize, string orientation)
        {
            bool landscape = string.Equals(orientation, "Landscape", StringComparison.OrdinalIgnoreCase);
            return (pageSize.ToUpperInvariant()) switch
            {
                "A5" => landscape ? PageSizes.A5.Landscape() : PageSizes.A5,
                "LETTER" => landscape ? PageSizes.Letter.Landscape() : PageSizes.Letter,
                _ => landscape ? PageSizes.A4.Landscape() : PageSizes.A4,
            };
        }
    }
}

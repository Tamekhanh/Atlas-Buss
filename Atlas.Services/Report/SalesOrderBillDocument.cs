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
            container
                .Page(page =>
                {
                    page.Size(MapPageSize(_data.PageSize, _data.Orientation));
                    page.Margin(40);
                    page.DefaultTextStyle(ts => ts.FontSize(10));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        // ===== Header: logo + thông tin công ty + tiêu đề bill =====
        void ComposeHeader(IContainer container)
        {
            var opts = _data.Options;

            container.PaddingBottom(10).Column(column =>
            {
                column.Item().Row(row =>
                {
                    // Cột trái: logo + thông tin công ty
                    row.RelativeItem().Column(col =>
                    {
                        if (opts.ShowLogo && _data.LogoBytes is { Length: > 0 })
                        {
                            col.Item().Height(50).MaxWidth(160).Image(_data.LogoBytes);
                        }

                        col.Item().Text(_data.CompanyName).FontSize(16).Bold();
                        col.Item().Text(_data.CompanyAddress ?? string.Empty).FontSize(9).FontColor(Colors.Grey.Darken1);
                        var contactLine = JoinNonEmpty(" | ",
                            _data.CompanyPhone, _data.CompanyEmail,
                            string.IsNullOrWhiteSpace(_data.CompanyTaxId) ? null : $"Tax ID: {_data.CompanyTaxId}");
                        if (!string.IsNullOrWhiteSpace(contactLine))
                        {
                            col.Item().Text(contactLine).FontSize(9).FontColor(Colors.Grey.Darken1);
                        }
                    });

                    // Cột phải: tiêu đề bill + số + ngày
                    row.ConstantItem(220).AlignRight().Column(col =>
                    {
                        col.Item().Text("SALES ORDER").FontSize(20).Bold();
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
                    column.Item().PaddingTop(6).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text(_data.HeaderNote).FontSize(9).Italic();
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
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);   // Product
                    columns.RelativeColumn(2);   // SKU
                    if (_data.Options.ShowWarehouseColumn)
                        columns.RelativeColumn(2); // Warehouse
                    columns.ConstantColumn(60);   // Qty
                    columns.ConstantColumn(80);   // Unit Price
                    columns.ConstantColumn(70);   // Discount
                    columns.ConstantColumn(70);   // Tax
                    columns.ConstantColumn(80);    // Line Total
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Product").Bold();
                    header.Cell().Element(CellStyle).Text("SKU").Bold();
                    if (_data.Options.ShowWarehouseColumn)
                        header.Cell().Element(CellStyle).Text("Warehouse").Bold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Qty").Bold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Unit Price").Bold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Discount").Bold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Tax").Bold();
                    header.Cell().Element(CellStyle).AlignRight().Text("Line Total").Bold();
                });

                foreach (var line in _data.Lines)
                {
                    table.Cell().Element(CellStyle).Text(line.ProductName);
                    table.Cell().Element(CellStyle).Text(line.Sku);
                    if (_data.Options.ShowWarehouseColumn)
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

                if (_data.Options.ShowTaxBreakdown)
                {
                    column.Item().Row(row =>
                    {
                        row.ConstantItem(160).Text("Total Tax").FontColor(Colors.Grey.Darken1);
                        row.ConstantItem(100).AlignRight().Text(Format(_data.TotalTax));
                    });
                }

                if (_data.Options.ShowGrandTotalBox)
                {
                    column.Item().PaddingTop(6);
                    column.Item().Background(Colors.Grey.Lighten3).Padding(8).Row(row =>
                    {
                        row.ConstantItem(160).Text($"Grand Total ({_data.CurrencyCode})").Bold();
                        row.ConstantItem(100).AlignRight().Text(Format(_data.GrandTotal)).Bold().FontSize(12);
                    });
                }
                else
                {
                    column.Item().PaddingTop(4);
                    column.Item().Row(row =>
                    {
                        row.ConstantItem(160).Text($"Grand Total ({_data.CurrencyCode})").Bold();
                        row.ConstantItem(100).AlignRight().Text(Format(_data.GrandTotal)).Bold();
                    });
                }

                if (_data.ExchangeRate != 1.0m && !string.IsNullOrWhiteSpace(_data.CurrencyCode))
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

                column.Item().AlignCenter().Text(t =>
                {
                    t.CurrentPageNumber();
                    t.Span(" / ");
                    t.TotalPages();
                });
            });
        }

        // ===== Helpers =====
        static IContainer CellStyle(IContainer container)
            => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4).PaddingHorizontal(4);

        static string Format(decimal value) => value.ToString("N2");

        static string? JoinNonEmpty(string separator, params string?[] parts)
            => string.Join(separator, parts.Where(p => !string.IsNullOrWhiteSpace(p)));

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

using Atlas.Core.DTOs;
using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Infrastructure;
using Microsoft.EntityFrameworkCore;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Atlas.Services.Report
{
    public class SalesOrderReportService : ISalesOrderReportService
    {
        private readonly AtlasDBContext _context;
        private readonly IBillTemplateRepository _templateRepository;
        private readonly IMyCompanyInfoRepository _companyRepository;
        private readonly IStorageProvider _storageProvider;

        public SalesOrderReportService(
            AtlasDBContext context,
            IBillTemplateRepository templateRepository,
            IMyCompanyInfoRepository companyRepository,
            IStorageProvider storageProvider)
        {
            _context = context;
            _templateRepository = templateRepository;
            _companyRepository = companyRepository;
            _storageProvider = storageProvider;
        }

        public async Task<SalesOrderReportData?> BuildReportDataAsync(int orderId, int templateId)
        {
            var order = await _context.SalesOrders
                .Include(o => o.Employee)
                .Include(o => o.Customer)!
                    .ThenInclude(p => p.Address)
                .Include(o => o.Customer)!
                    .ThenInclude(p => p.Contact)
                .Include(o => o.SalesOrderDetails)
                    .ThenInclude(d => d.Variant)!
                        .ThenInclude(v => v.Product)
                .Include(o => o.SalesOrderDetails)
                    .ThenInclude(d => d.Warehouse)
                .Include(o => o.OrderStatus)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId && !o.IsDeleted);

            if (order == null) return null;

            var template = await _templateRepository.GetByIdAsync(templateId)
                ?? await _templateRepository.GetDefaultAsync();

            var company = await _companyRepository.GetAsync();

            // Lấy mã tiền tệ (không rely navigation tự include).
            Currencies? currency = null;
            if (order.CurrencyId > 0)
            {
                currency = await _context.Currencies
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == order.CurrencyId);
            }

            var options = ParseOptions(template?.OptionsJson);

            var data = new SalesOrderReportData
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                StatusName = order.OrderStatus?.StatusName ?? string.Empty,
                CompanyName = company?.CompanyName ?? string.Empty,
                CompanyAddress = company?.Address,
                CompanyPhone = company?.PhoneNumber,
                CompanyEmail = company?.Email,
                CompanyTaxId = company?.TaxId,
                CustomerName = order.Customer?.DisplayName ?? string.Empty,
                CustomerAddress = FormatAddress(order.Customer?.Address),
                CustomerPhone = order.Customer?.Contact?.Phone,
                CustomerEmail = order.Customer?.Contact?.Email,
                EmployeeName = order.Employee?.FullName ?? string.Empty,
                CurrencyCode = currency?.CurrencyCode ?? string.Empty,
                ExchangeRate = order.ExchangeRate,
                PageSize = template?.PageSize ?? "A4",
                Orientation = template?.Orientation ?? "Portrait",
                Options = options,
                HeaderNote = template?.HeaderNote,
                FooterNote = template?.FooterNote,
            };

            // Logo
            if (options.ShowLogo && company?.Logo?.ImageUrl is { Length: > 0 } logoUrl)
            {
                data.LogoBytes = await SafeReadBytesAsync(logoUrl);
            }

            // Lines
            foreach (var d in order.SalesOrderDetails.Where(x => !x.IsDeleted).OrderBy(d => d.Id))
            {
                data.Lines.Add(new SalesOrderReportLine
                {
                    ProductName = d.Variant?.Product?.ProductName ?? string.Empty,
                    Sku = d.Variant?.SKU ?? string.Empty,
                    WarehouseName = d.Warehouse?.WarehouseName ?? d.WarehouseId.ToString(),
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Discount = d.Discount,
                    TaxAmount = d.TaxAmount,
                    LineTotal = (d.Quantity * d.UnitPrice) - d.Discount + d.TaxAmount,
                });
            }

            data.SubTotal = data.Lines.Sum(l => l.Quantity * l.UnitPrice);
            data.TotalDiscount = data.Lines.Sum(l => l.Discount);
            data.TotalTax = data.Lines.Sum(l => l.TaxAmount);
            data.GrandTotal = order.TotalAmount;

            return data;
        }

        public byte[] RenderPdf(SalesOrderReportData data)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var document = new SalesOrderBillDocument(data);
            return document.GeneratePdf();
        }

        // ===== Helpers =====
        private static BillTemplateOptions ParseOptions(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new BillTemplateOptions();

            try
            {
                var opts = JsonSerializer.Deserialize<BillTemplateOptions>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return opts ?? new BillTemplateOptions();
            }
            catch
            {
                return new BillTemplateOptions();
            }
        }

        private static string? FormatAddress(Addresses? addr)
        {
            if (addr == null) return null;
            var parts = new[] { addr.Street, addr.City, addr.State, addr.Country }
                .Where(p => !string.IsNullOrWhiteSpace(p));
            var joined = string.Join(", ", parts);
            return string.IsNullOrWhiteSpace(joined) ? null : joined;
        }

        private async Task<byte[]?> SafeReadBytesAsync(string relativePath)
        {
            try
            {
                using var stream = await _storageProvider.GetFileAsync(relativePath!);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }
    }
}

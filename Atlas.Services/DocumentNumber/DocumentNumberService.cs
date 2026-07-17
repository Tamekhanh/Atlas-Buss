using Atlas.Core.Interfaces;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Atlas.Services
{
    /// <summary>
    /// Sinh mã số tham chiếu tuần tự cho các chứng từ (PO, SO, ...) theo định dạng
    /// PREFIX-YYYY-NNN, trong đó NNN là số thứ tự tăng dần theo năm (bắt đầu lại mỗi năm)
    /// và được zero-pad tối thiểu 3 chữ số.
    ///
    /// Cách tính: lấy danh sách các số đã tồn tại khớp với prefix + năm hiện tại,
    /// tìm giá trị hậu tố lớn nhất rồi cộng 1. Không dùng bảng sequence riêng, tận dụng
    /// cột UNIQUE của bảng để tránh trùng số (nếu có race thì bản ghi thua sẽ thất bại).
    /// </summary>
    public interface IDocumentNumberService
    {
        /// <summary>Sinh số Purchase Order tiếp theo (PO-YYYY-NNN).</summary>
        Task<string> GeneratePurchaseOrderNumberAsync();

        /// <summary>Sinh số Sales Order tiếp theo (SO-YYYY-NNN).</summary>
        Task<string> GenerateSalesOrderNumberAsync();
    }

    public class DocumentNumberService : IDocumentNumberService
    {
        // Zero pad tối thiểu 3 chữ số (001, 002, ...); tự mở rộng lên 4 chữ số khi vượt 999.
        private const int MinDigits = 3;

        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly ISalesOrderRepository _salesOrderRepository;

        public DocumentNumberService(
            IPurchaseOrderRepository purchaseOrderRepository,
            ISalesOrderRepository salesOrderRepository)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _salesOrderRepository = salesOrderRepository;
        }

        public Task<string> GeneratePurchaseOrderNumberAsync()
        {
            return BuildNextNumberAsync(_purchaseOrderRepository.GetAllNumbersAsync(), "PO");
        }

        public Task<string> GenerateSalesOrderNumberAsync()
        {
            return BuildNextNumberAsync(_salesOrderRepository.GetAllNumbersAsync(), "SO");
        }

        private async Task<string> BuildNextNumberAsync(System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<string>> numbersTask, string prefix)
        {
            var year = DateTime.Now.Year;
            var yearText = year.ToString(CultureInfo.InvariantCulture);
            var prefixWithYear = $"{prefix}-{yearText}-";

            var numbers = await numbersTask;

            // Lọc các số thuộc cùng prefix + năm, rồi tìm hậu tố số lớn nhất.
            int maxSequence = 0;
            if (numbers != null)
            {
                foreach (var raw in numbers)
                {
                    if (string.IsNullOrWhiteSpace(raw)) continue;
                    var current = raw.Trim();

                    if (!current.StartsWith(prefixWithYear, StringComparison.OrdinalIgnoreCase)) continue;

                    var suffix = current.Substring(prefixWithYear.Length);
                    if (int.TryParse(suffix, NumberStyles.None, CultureInfo.InvariantCulture, out var seq) && seq > maxSequence)
                    {
                        maxSequence = seq;
                    }
                }
            }

            var nextSequence = maxSequence + 1;
            return $"{prefixWithYear}{nextSequence.ToString($"D{MinDigits}", CultureInfo.InvariantCulture)}";
        }
    }
}

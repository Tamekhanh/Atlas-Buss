using Atlas.Core.DTOs;
using System.Threading.Tasks;

namespace Atlas.Services
{
    /// <summary>
    /// Sinh tài liệu PDF in bill cho Sales Order dựa trên mẫu (BillTemplate) người dùng chọn.
    /// </summary>
    public interface ISalesOrderReportService
    {
        /// <summary>
        /// Tổng hợp dữ liệu từ DB theo orderId + templateId (mẫu đã lưu).
        /// </summary>
        Task<SalesOrderReportData?> BuildReportDataAsync(int orderId, int templateId);

        /// <summary>
        /// Tổng hợp dữ liệu từ DB theo orderId + các tùy chọn chưa lưu (dùng cho Preview form tạo/sửa mẫu).
        /// </summary>
        Task<SalesOrderReportData?> BuildReportDataAsync(
            int orderId,
            BillTemplateOptions options,
            string pageSize,
            string orientation,
            string? headerNote,
            string? footerNote);

        /// <summary>
        /// Render dữ liệu đã tổng hợp thành mảng bytes PDF.
        /// </summary>
        byte[] RenderPdf(SalesOrderReportData data);
    }
}

using Atlas.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface ISalesOrderRepository
    {
        Task<IEnumerable<SalesOrder>> GetAllAsync();
        Task<SalesOrder?> GetByIdAsync(int id);
        Task<SalesOrder?> GetByOrderNumberAsync(string orderNumber);
        Task<IEnumerable<string>> GetAllNumbersAsync();

        /// <summary>
        /// Danh sách nhẹ (id + số + ngày + tên khách) để nạp dropdown chọn order cho preview bill.
        /// </summary>
        Task<IEnumerable<(int Id, string OrderNumber, DateTime OrderDate, string CustomerName)>> GetOrderListAsync();

        Task<bool> AddAsync(SalesOrder order);
        Task<bool> UpdateAsync(SalesOrder order);
        Task<bool> DeleteAsync(int id);
    }
}

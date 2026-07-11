using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface ISalesOrderBillRepository
    {
        Task<bool> AddAsync(SalesOrderBill bill);
        Task<IEnumerable<SalesOrderBill>> GetByOrderIdAsync(int orderId);
        Task<SalesOrderBill?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}

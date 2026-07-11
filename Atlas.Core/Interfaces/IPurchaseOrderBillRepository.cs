using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IPurchaseOrderBillRepository
    {
        Task<bool> AddAsync(PurchaseOrderBill bill);
        Task<IEnumerable<PurchaseOrderBill>> GetByOrderIdAsync(int orderId);
        Task<PurchaseOrderBill?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}

using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task<IEnumerable<PurchaseOrder>> GetAllAsync();
        Task<PurchaseOrder> GetByIdAsync(int id);
        Task<PurchaseOrder> GetByPONumberAsync(string poNumber);
        Task<bool> AddAsync(PurchaseOrder order);
        Task<bool> UpdateAsync(PurchaseOrder order);
        Task<bool> DeleteAsync(int id);
    }
}

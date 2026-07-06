using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Services
{
    public interface IPurchaseOrderService
    {
        Task<IEnumerable<PurchaseOrder>> GetAllAsync();
        Task<PurchaseOrder?> GetByIdAsync(int id);
        Task<PurchaseOrder?> GetByPONumberAsync(string poNumber);
        Task<bool> CreateAsync(PurchaseOrder order);
        Task<bool> UpdateAsync(PurchaseOrder order);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, int newStatusId);
    }
}
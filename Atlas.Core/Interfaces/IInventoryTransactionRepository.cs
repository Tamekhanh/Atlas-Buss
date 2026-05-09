using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IInventoryTransactionRepository
    {
        Task<IEnumerable<InventoryTransaction>> GetAllAsync();
        Task<InventoryTransaction> GetByIdAsync(long id);
        Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(int productId);
        Task<IEnumerable<InventoryTransaction>> GetByWarehouseIdAsync(int warehouseId);
        Task<bool> AddAsync(InventoryTransaction transaction);
    }
}

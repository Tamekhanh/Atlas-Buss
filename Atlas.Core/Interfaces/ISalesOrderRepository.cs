using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface ISalesOrderRepository
    {
        Task<IEnumerable<SalesOrder>> GetAllAsync();
        Task<SalesOrder> GetByIdAsync(int id);
        Task<SalesOrder> GetByOrderNumberAsync(string orderNumber);
        Task<bool> AddAsync(SalesOrder order);
        Task<bool> UpdateAsync(SalesOrder order);
        Task<bool> DeleteAsync(int id);
    }
}

using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IWarehouseRepository
    {
        Task<IEnumerable<Warehouse>> GetAllAsync();
        Task<Warehouse> GetByIdAsync(int id);
        Task<bool> AddAsync(Warehouse warehouse);
        Task<bool> UpdateAsync(Warehouse warehouse);
        Task<bool> DeleteAsync(int id);
    }
}

using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface ITaxRepository
    {
        Task<IEnumerable<Tax>> GetAllAsync();
        Task<IEnumerable<Tax>> GetActiveAsync();
        Task<Tax> GetByIdAsync(int id);
        Task<bool> AddAsync(Tax tax);
        Task<bool> UpdateAsync(Tax tax);
        Task<bool> DeleteAsync(int id);
    }
}

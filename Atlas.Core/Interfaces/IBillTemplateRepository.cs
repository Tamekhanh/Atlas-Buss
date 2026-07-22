using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IBillTemplateRepository
    {
        Task<IEnumerable<BillTemplates>> GetAllAsync();
        Task<BillTemplates?> GetByIdAsync(int id);
        Task<BillTemplates?> GetDefaultAsync();
        Task<bool> AddAsync(BillTemplates template);
        Task<bool> UpdateAsync(BillTemplates template);
        Task<bool> DeleteAsync(int id);
        Task<bool> ClearDefaultAsync();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
// Đảm bảo using chính xác lớp Entity
using Atlas.Core.Entities; 

namespace Atlas.Core.Interfaces
{
    public interface ICategoryService
    {
        // Sử dụng đường dẫn đầy đủ để tránh nhầm lẫn
        Task<IEnumerable<Atlas.Core.Entities.Category>> GetAllAsync();
        Task<Atlas.Core.Entities.Category> GetByIdAsync(int id);
        Task<Atlas.Core.Entities.Category?> FindByNameAsync(string categoryName);
        Task<bool> AddAsync(Atlas.Core.Entities.Category category);
        Task<bool> UpdateAsync(Atlas.Core.Entities.Category category);
        Task<bool> DeleteAsync(int id);
    }
}
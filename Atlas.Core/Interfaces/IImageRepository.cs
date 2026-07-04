using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IImageRepository
    {
        Task<bool> AddAsync(Images image);
        Task<Images?> GetByIdAsync(int id);
        Task<IEnumerable<Images>> GetAllAsync();
        Task<bool> DeleteAsync(int id);
        Task<Images?> GetByUrlAsync(string url);
    }
}
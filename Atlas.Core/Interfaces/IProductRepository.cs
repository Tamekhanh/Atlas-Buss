using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Products>> GetAllAsync();
        Task<Products> GetByIdAsync(int id);
        Task<bool> AddAsync(Products product);
        Task<bool> UpdateAsync(Products product);
        Task<bool> DeleteAsync(int id);
    }
}

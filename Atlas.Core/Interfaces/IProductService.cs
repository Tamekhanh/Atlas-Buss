using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    // Đây chỉ là "bản hợp đồng" liệt kê các chức năng sẽ có
    public interface IProductService
    {
        Task<IEnumerable<Products>> GetAllProductsAsync();
        Task<Products> GetProductByIdAsync(int id);
        Task<bool> CreateProductAsync(Products product);
        Task<bool> UpdateProductAsync(Products product);
        Task<bool> DeleteProductAsync(int id);
    }
}
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
        Task<bool> CreateProductAsync(Products product, IEnumerable<int>? categoryIds = null, string? newCategoryName = null);
        Task<bool> UpdateProductAsync(Products product, IEnumerable<int>? categoryIds = null, string? newCategoryName = null);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<Products>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Products>> GetProductFilterAsync(string? searchTerm = null, string? category = null, bool? isActive = null, bool? onSale = null);
    }
}
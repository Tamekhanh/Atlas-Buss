using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IProductRepository
    {
        // --- Quản lý Sản phẩm Cha (Parent Product) ---
        Task<IEnumerable<Products>> GetAllAsync(int pageNumber, int pageSize);
        Task<Products> GetByIdAsync(int id); // Nên Include Variants và ProductDetail
        Task<bool> AddAsync(Products product);
        Task<bool> UpdateAsync(Products product);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Products>> GetProductFilterAsync(string? searchTerm, string? category = null, bool? isActive = null, bool? onSale = null);
        Task<IEnumerable<Products>> SearchByNameAsync(string searchTerm, int pageNumber, int pageSize);

        // --- Quản lý Biến thể (Product Variants) ---
        Task<IEnumerable<ProductVariant>> GetVariantsByProductIdAsync(int productId);
        Task<ProductVariant> GetVariantByIdAsync(int variantId);
        Task<bool> AddVariantAsync(ProductVariant variant);
        Task<bool> UpdateVariantAsync(ProductVariant variant);
        Task<bool> DeleteVariantAsync(int variantId);
        
        // --- Quản lý Thuộc tính (Attributes) ---
        // Có thể tách ra IAttributeRepository nếu danh sách thuộc tính quá lớn
        Task<IEnumerable<AttributeType>> GetAllAttributeTypesAsync();
        Task<IEnumerable<AttributeValue>> GetValuesByAttributeTypeIdAsync(int attributeTypeId);
    }
}
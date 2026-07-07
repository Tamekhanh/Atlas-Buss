using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IProductService
    {
        // --- Xử lý Sản phẩm Cha ---
        Task<IEnumerable<Products>> GetAllProductsAsync(int pageNumber = 0, int pageSize = 10);
        Task<Products> GetProductByIdAsync(int id);

        // Tạo sản phẩm kèm theo danh sách biến thể và phân loại
        Task<bool> CreateProductAsync(Products product, IEnumerable<int>? categoryIds = null, IEnumerable<int>? imageIds = null, IEnumerable<ProductVariant>? variants = null);
        Task<bool> UpdateProductAsync(Products product, IEnumerable<int>? categoryIds = null, IEnumerable<int>? imageIds = null, IEnumerable<ProductVariant>? variants = null);
        Task<bool> DeleteProductAsync(int id);

        Task<IEnumerable<Products>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Products>> GetProductFilterAsync(string? searchTerm = null, string? category = null, bool? isActive = null, bool? onSale = null);

        // --- Xử lý Biến thể (Variants) ---
        Task<IEnumerable<ProductVariant>> GetVariantsOfProductAsync(int productId);

        // Cập nhật giá hoặc trạng thái cho một SKU cụ thể
        Task<bool> UpdateVariantAsync(ProductVariant variant);

        // Xóa một biến thể (ví dụ: ngừng bán size L màu Đỏ)
        Task<bool> DeleteVariantAsync(int variantId);

        // --- Xử lý Thuộc tính (Attributes) ---
        Task<IEnumerable<AttributeType>> GetAvailableAttributeTypesAsync();
        Task<IEnumerable<AttributeValue>> GetAttributeValuesAsync(int attributeTypeId);
    }
}
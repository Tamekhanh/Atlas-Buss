using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Services.Inventory
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        // =============================================
        // 1. QUẢN LÝ SẢN PHẨM CHA (PARENT PRODUCT)
        // =============================================

        public async Task<bool> CreateProductAsync(Products product, IEnumerable<int>? categoryIds = null, IEnumerable<ProductVariant>? variants = null)
        {
            // 1. Kiểm tra logic cơ bản (Sử dụng BaseSalePrice thay vì SalePrice)
            if (product.BaseSalePrice <= 0) return false;
            if (product.UnitId <= 0) return false;

            // 2. Xử lý Danh mục (Categories) - Giữ logic cũ của bạn
            var distinctCategoryIds = (categoryIds ?? Enumerable.Empty<int>())
                .Where(categoryId => categoryId > 0)
                .Distinct()
                .ToList();

            // Lưu ý: Nếu bạn muốn hỗ trợ tạo category mới từ string trong hàm này, 
            // hãy thêm tham số 'string? newCategoryName' vào Interface và hàm này.

            if (distinctCategoryIds.Count > 0)
            {
                product.CategoryProducts = distinctCategoryIds
                    .Select(categoryId => new CategoryProduct
                    {
                        CategoryId = categoryId
                    })
                    .ToList();
            }

            // 3. Xử lý Biến thể (Variants)
            // Nếu variants được truyền vào, chúng sẽ được lưu cùng với product (vì là Navigation Property)
            if (variants != null)
            {
                product.Variants = variants.ToList();
            }

            return await _productRepository.AddAsync(product);
        }

        public async Task<bool> UpdateProductAsync(Products product, IEnumerable<int>? categoryIds = null)
        {
            if (product.BaseSalePrice <= 0) return false;

            var distinctCategoryIds = (categoryIds ?? Enumerable.Empty<int>())
                .Where(categoryId => categoryId > 0)
                .Distinct()
                .ToList();

            if (distinctCategoryIds.Count > 0)
            {
                product.CategoryProducts = distinctCategoryIds
                    .Select(categoryId => new CategoryProduct
                    {
                        CategoryId = categoryId
                    })
                    .ToList();
            }

            return await _productRepository.UpdateAsync(product);
        }

        public async Task<IEnumerable<Products>> GetAllProductsAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _productRepository.GetAllAsync(pageNumber, pageSize);
        }

        public async Task<IEnumerable<Products>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync(1, int.MaxValue);
        }

        public async Task<Products> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Products>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await _productRepository.GetAllAsync(1, int.MaxValue);
            }
            return await _productRepository.SearchByNameAsync(searchTerm.Trim(), 1, int.MaxValue);
        }

        public async Task<IEnumerable<Products>> GetProductFilterAsync(string? searchTerm = null, string? category = null, bool? isActive = null, bool? onSale = null)
        {
            return await _productRepository.GetProductFilterAsync(
                string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim(),
                category,
                isActive,
                onSale);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        // =============================================
        // 2. QUẢN LÝ BIẾN THỂ (VARIANTS)
        // =============================================

        public async Task<IEnumerable<ProductVariant>> GetVariantsOfProductAsync(int productId)
        {
            return await _productRepository.GetVariantsByProductIdAsync(productId);
        }

        public async Task<bool> UpdateVariantAsync(ProductVariant variant)
        {
            if (variant == null) return false;
            return await _productRepository.UpdateVariantAsync(variant);
        }

        public async Task<bool> DeleteVariantAsync(int variantId)
        {
            return await _productRepository.DeleteVariantAsync(variantId);
        }

        // =============================================
        // 3. QUẢN LÝ THUỘC TÍNH (ATTRIBUTES)
        // =============================================

        public async Task<IEnumerable<AttributeType>> GetAvailableAttributeTypesAsync()
        {
            return await _productRepository.GetAllAttributeTypesAsync();
        }

        public async Task<IEnumerable<AttributeValue>> GetAttributeValuesAsync(int attributeTypeId)
        {
            return await _productRepository.GetValuesByAttributeTypeIdAsync(attributeTypeId);
        }
    }
}
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

        // CẬP NHẬT: Thêm IEnumerable<int>? imageIds vào tham số
        public async Task<bool> CreateProductAsync(Products product, IEnumerable<int>? categoryIds = null, IEnumerable<int>? imageIds = null, IEnumerable<ProductVariant>? variants = null)
        {
            // 1. Kiểm tra logic cơ bản
            if (product.BaseSalePrice <= 0) return false;
            if (product.UnitId <= 0) return false;

            // 2. Xử lý Danh mục (Categories)
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

            // 3. Xử lý Hình ảnh (Images) - MỚI THÊM
            var distinctImageIds = (imageIds ?? Enumerable.Empty<int>())
                .Where(imageId => imageId > 0)
                .Distinct()
                .ToList();

            if (distinctImageIds.Count > 0)
            {
                product.ProductImages = distinctImageIds
                    .Select(imageId => new ProductImages
                    {
                        ImageId = imageId
                    })
                    .ToList();
            }

            // 4. Xử lý Biến thể (Variants)
            if (variants != null)
            {
                product.Variants = variants.ToList();
            }

            return await _productRepository.AddAsync(product);
        }

        // CẬP NHẬT: Thêm IEnumerable<int>? imageIds vào tham số
        public async Task<bool> UpdateProductAsync(Products product, IEnumerable<int>? categoryIds = null, IEnumerable<int>? imageIds = null)
        {
            if (product.BaseSalePrice <= 0) return false;

            // 1. Xử lý Danh mục
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

            // 2. Xử lý Hình ảnh - MỚI THÊM
            var distinctImageIds = (imageIds ?? Enumerable.Empty<int>())
                .Where(imageId => imageId > 0)
                .Distinct()
                .ToList();

            if (distinctImageIds.Count > 0)
            {
                product.ProductImages = distinctImageIds
                    .Select(imageId => new ProductImages
                    {
                        ImageId = imageId
                    })
                    .ToList();
            }
            else
            {
                // Nếu imageIds truyền vào là null hoặc rỗng, 
                // có thể hiểu là xóa hết ảnh của sản phẩm
                product.ProductImages = new List<ProductImages>();
            }

            return await _productRepository.UpdateAsync(product);
        }

        // ... các hàm GetAllProductsAsync, GetProductByIdAsync, SearchByNameAsync, GetProductFilterAsync giữ nguyên ...

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
using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Services.Inventory
{
    // Class này kế thừa và thực thi chi tiết các hàm từ IProductService
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        // Tiêm Repository vào qua Constructor (DI)
        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> CreateProductAsync(Products product, IEnumerable<int>? categoryIds = null, string? newCategoryName = null)
        {
            // Viết logic kiểm tra (ví dụ: cấm tạo giá âm)
            if (product.SalePrice <= 0) return false;
            if (product.UnitId <= 0) return false;

            var distinctCategoryIds = (categoryIds ?? Enumerable.Empty<int>())
                .Where(categoryId => categoryId > 0)
                .Distinct()
                .ToList();

            if (!string.IsNullOrWhiteSpace(newCategoryName))
            {
                var trimmedCategoryName = newCategoryName.Trim();
                var existingCategory = await _categoryRepository.FindByNameAsync(trimmedCategoryName);

                if (existingCategory is null)
                {
                    var createdCategory = new Category
                    {
                        CategoryName = trimmedCategoryName
                    };

                    if (!await _categoryRepository.AddAsync(createdCategory))
                    {
                        return false;
                    }

                    distinctCategoryIds.Add(createdCategory.Id);
                }
                else
                {
                    distinctCategoryIds.Add(existingCategory.Id);
                }
            }

            if (distinctCategoryIds.Count > 0)
            {
                product.CategoryProducts = distinctCategoryIds
                    .Distinct()
                    .Select(categoryId => new CategoryProduct
                    {
                        CategoryId = categoryId
                    })
                    .ToList();
            }

            return await _productRepository.AddAsync(product);
        }

        public async Task<IEnumerable<Products>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Products>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await _productRepository.GetAllAsync();
            }

            return await _productRepository.SearchByNameAsync(searchTerm.Trim());
        }

        public async Task<Products> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Products>> GetProductFilterAsync(string? searchTerm = null, string? category = null, bool? isActive = null, bool? onSale = null)
        {
            return await _productRepository.GetProductFilterAsync(
                string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim(),
                category,
                isActive,
                onSale);
        }

        public async Task<bool> UpdateProductAsync(Products product)
        {
            if (product.SalePrice <= 0) return false;

            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }
    }
}
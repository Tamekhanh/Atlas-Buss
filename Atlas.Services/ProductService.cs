using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Services.Inventory
{
    // Class này kế thừa và thực thi chi tiết các hàm từ IProductService
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        // Tiêm Repository vào qua Constructor (DI)
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<bool> CreateProductAsync(Products product)
        {
            // Viết logic kiểm tra (ví dụ: cấm tạo giá âm)
            if (product.SalePrice <= 0) return false;

            return await _productRepository.AddAsync(product);
        }

        public async Task<IEnumerable<Products>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Products> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
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
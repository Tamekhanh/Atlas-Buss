using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AtlasDBContext _context;

        public ProductRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Products>> GetAllAsync()
        {
            return await _context.Products
                .Include(product => product.Employee)
                .ThenInclude(employee => employee.Person)
                .Include(product => product.CategoryProducts)
                .ThenInclude(categoryProduct => categoryProduct.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Products>> GetProductFilterAsync(string? searchTerm, string? category = null, bool? isActive = null, bool? onSale = null)
        {
            var query = _context.Products
                .Include(product => product.Employee)
                .ThenInclude(employee => employee.Person)
                .Include(product => product.CategoryProducts)
                .ThenInclude(categoryProduct => categoryProduct.Category)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(product => product.ProductName.Contains(searchTerm) || product.ProductCode.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(product => _context.CategoryProducts.Any(categoryProduct =>
                    categoryProduct.ProductId == product.Id &&
                    categoryProduct.Category != null &&
                    categoryProduct.Category.CategoryName.Contains(category)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(product => product.IsActive == isActive.Value);
            }

            if (onSale.HasValue)
            {
                query = query.Where(product => product.Onsale == onSale.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Products>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync();
            }

            var term = searchTerm.Trim();
            return await _context.Products
                .Include(product => product.Employee)
                .ThenInclude(employee => employee.Person)
                .Include(product => product.CategoryProducts)
                .ThenInclude(categoryProduct => categoryProduct.Category)
                .AsNoTracking()
                .Where(product => product.ProductName.Contains(term) || product.ProductCode.Contains(term))
                .ToListAsync();
        }

        public async Task<Products> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(product => product.Employee)
                .ThenInclude(employee => employee.Person)
                .Include(product => product.CategoryProducts)
                .ThenInclude(categoryProduct => categoryProduct.Category)
                .Include(product => product.ProductDetail)
                .AsNoTracking()
                .FirstOrDefaultAsync(product => product.Id == id);
        }

        public async Task<bool> AddAsync(Products product)
        {
            await _context.Products.AddAsync(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Products product)
        {
            var existingProduct = await _context.Products
                .Include(current => current.ProductDetail)
                .Include(current => current.CategoryProducts)
                .FirstOrDefaultAsync(current => current.Id == product.Id);

            if (existingProduct is null)
            {
                return false;
            }

            existingProduct.ProductName = product.ProductName;
            existingProduct.ProductCode = product.ProductCode;
            existingProduct.UnitId = product.UnitId;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.SalePrice = product.SalePrice;
            existingProduct.CostPrice = product.CostPrice;
            existingProduct.Barcode = product.Barcode;
            existingProduct.IsActive = product.IsActive;
            existingProduct.Onsale = product.Onsale;
            existingProduct.UpdatedAt = DateTime.Now;

            if (product.ProductDetail is not null)
            {
                if (existingProduct.ProductDetail is null)
                {
                    existingProduct.ProductDetail = new ProductDetails
                    {
                        ProductId = existingProduct.Id
                    };
                }

                existingProduct.ProductDetail.ProductDescription = product.ProductDetail.ProductDescription;
                existingProduct.ProductDetail.Weight = product.ProductDetail.Weight;
                existingProduct.ProductDetail.WarrantyPeriod = product.ProductDetail.WarrantyPeriod;
                existingProduct.ProductDetail.Dimensions = product.ProductDetail.Dimensions;
                existingProduct.ProductDetail.Manufacturer = product.ProductDetail.Manufacturer;
            }

            existingProduct.CategoryProducts.Clear();
            if (product.CategoryProducts is not null)
            {
                foreach (var categoryProduct in product.CategoryProducts)
                {
                    existingProduct.CategoryProducts.Add(new CategoryProduct
                    {
                        ProductId = existingProduct.Id,
                        CategoryId = categoryProduct.CategoryId
                    });
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null)
            {
                return false;
            }

            _context.Products.Remove(product);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
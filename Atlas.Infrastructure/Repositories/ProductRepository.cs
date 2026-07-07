using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AtlasDBContext _context;

        public ProductRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Products>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .Include(product => product.Employee)
                .Include(product => product.CategoryProducts)
                    .ThenInclude(categoryProduct => categoryProduct.Category)
                .Include(product => product.Unit)
                // THÊM DÒNG NÀY ĐỂ LẤY ẢNH
                .Include(product => product.ProductImages)
                    .ThenInclude(pi => pi.Image)
                .AsNoTracking()
                .OrderBy(product => product.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Products>> GetAllWithVariantsAsync()
        {
            return await _context.Products
                .Include(product => product.Variants)
                .AsNoTracking()
                .OrderBy(product => product.ProductName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Products>> GetProductFilterAsync(string? searchTerm, string? category = null, bool? isActive = null, bool? onSale = null)
        {
            var query = _context.Products
                .Include(product => product.Employee)
                .Include(product => product.CategoryProducts)
                    .ThenInclude(categoryProduct => categoryProduct.Category)
                .Include(product => product.Unit)
                // SỬA ĐOẠN NÀY: Bỏ .Include(product => product.ImageUrl) vì ImageUrl là string
                .Include(product => product.ProductImages)
                    .ThenInclude(pi => pi.Image)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(product => product.ProductName.Contains(searchTerm) || product.ProductCode.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(product => _context.CategoryProducts.Any(cp =>
                    cp.ProductId == product.Id &&
                    cp.Category != null &&
                    cp.Category.CategoryName.Contains(category)));
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

        public async Task<IEnumerable<Products>> SearchByNameAsync(string searchTerm, int pageNumber, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync(pageNumber, pageSize);
            }

            var term = searchTerm.Trim();
            return await _context.Products
                .Include(product => product.Employee)
                .Include(product => product.CategoryProducts)
                    .ThenInclude(categoryProduct => categoryProduct.Category)
                // THÊM DÒNG NÀY ĐỂ LẤY ẢNH
                .Include(product => product.ProductImages)
                    .ThenInclude(pi => pi.Image)
                .AsNoTracking()
                .Where(product => product.ProductName.Contains(term) || product.ProductCode.Contains(term))
                .OrderBy(product => product.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Products?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(product => product.Employee)
                .Include(product => product.CategoryProducts)
                    .ThenInclude(categoryProduct => categoryProduct.Category)
                .Include(product => product.ProductDetail)
                .Include(product => product.Unit)
                // THÊM DÒNG NÀY ĐỂ LẤY ẢNH
                .Include(product => product.ProductImages)
                    .ThenInclude(pi => pi.Image)
                .Include(product => product.Variants)
                    .ThenInclude(v => v.AttributeMappings)
                        .ThenInclude(m => m.AttributeValue)
                            .ThenInclude(av => av.AttributeType)
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
                .Include(current => current.ProductImages)
                .FirstOrDefaultAsync(current => current.Id == product.Id);

            if (existingProduct is null) return false;

            existingProduct.ProductName = product.ProductName;
            existingProduct.ProductCode = product.ProductCode;
            existingProduct.UnitId = product.UnitId;
            existingProduct.BaseSalePrice = product.BaseSalePrice;
            existingProduct.BaseCostPrice = product.BaseCostPrice;
            existingProduct.Barcode = product.Barcode;
            existingProduct.IsActive = product.IsActive;
            existingProduct.Onsale = product.Onsale;
            existingProduct.UpdatedAt = DateTime.Now;

            if (product.ProductDetail is not null)
            {
                if (existingProduct.ProductDetail is null)
                {
                    existingProduct.ProductDetail = new ProductDetails { ProductId = existingProduct.Id };
                }

                existingProduct.ProductDetail.ProductDescription = product.ProductDetail.ProductDescription;
                existingProduct.ProductDetail.Weight = product.ProductDetail.Weight;
                existingProduct.ProductDetail.WarrantyPeriod = product.ProductDetail.WarrantyPeriod;
                existingProduct.ProductDetail.Dimensions = product.ProductDetail.Dimensions;
                existingProduct.ProductDetail.Manufacturer = product.ProductDetail.Manufacturer;
            }

            existingProduct.ProductImages.Clear();
            if (product.ProductImages != null && product.ProductImages.Any())
            {
                existingProduct.ProductImages.Clear();
                foreach (var pi in product.ProductImages)
                {
                    existingProduct.ProductImages.Add(new ProductImages
                    {
                        ProductId = existingProduct.Id,
                        ImageId = pi.ImageId
                    });
                }
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is null) return false;

            _context.Products.Remove(product);
            return await _context.SaveChangesAsync() > 0;
        }

        // =============================================
        // 2. QUẢN LÝ BIẾN THỂ (VARIANTS)
        // =============================================

        public async Task<IEnumerable<ProductVariant>> GetVariantsByProductIdAsync(int productId)
        {
            return await _context.ProductVariants
                .Include(v => v.AttributeMappings)
                    .ThenInclude(m => m.AttributeValue)
                .Where(v => v.ProductId == productId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ProductVariant?> GetVariantByIdAsync(int variantId)
        {
            return await _context.ProductVariants
                .Include(v => v.AttributeMappings)
                    .ThenInclude(m => m.AttributeValue)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == variantId);
        }

        public async Task<bool> AddVariantAsync(ProductVariant variant)
        {
            await _context.ProductVariants.AddAsync(variant);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateVariantAsync(ProductVariant variant)
        {
            var existing = await _context.ProductVariants.FindAsync(variant.Id);
            if (existing == null) return false;

            existing.SKU = variant.SKU;
            existing.VariantPrice = variant.VariantPrice;
            existing.VariantCost = variant.VariantCost;
            existing.IsActive = variant.IsActive;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteVariantAsync(int variantId)
        {
            var variant = await _context.ProductVariants.FindAsync(variantId);
            if (variant == null) return false;

            _context.ProductVariants.Remove(variant);
            return await _context.SaveChangesAsync() > 0;
        }

        // =============================================
        // 3. QUẢN LÝ THUỘC TÍNH (ATTRIBUTES)
        // =============================================

        public async Task<IEnumerable<AttributeType>> GetAllAttributeTypesAsync()
        {
            return await _context.AttributeTypes
                // Đảm bảo Navigation Property trong AttributeTypes là `Values` hoặc `AttributeValues`
                .Include(t => t.Values)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<AttributeValue>> GetValuesByAttributeTypeIdAsync(int attributeTypeId)
        {
            return await _context.AttributeValues
                .Where(v => v.AttributeTypeId == attributeTypeId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
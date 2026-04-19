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
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Products> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(product => product.Employee)
                .ThenInclude(employee => employee.Person)
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
            _context.Products.Update(product);
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
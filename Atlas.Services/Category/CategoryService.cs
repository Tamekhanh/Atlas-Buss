using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Atlas.Core.Interfaces;
using Atlas.Infrastructure; 
// Đảm bảo dùng đúng Entity
using Atlas.Core.Entities; 

namespace Atlas.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly AtlasDBContext _context;

        public CategoryService(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Atlas.Core.Entities.Category>> GetAllAsync()
        {
            return await _context.Categories
                                 .AsNoTracking() 
                                 .ToListAsync();
        }

        public async Task<Atlas.Core.Entities.Category> GetByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            return category;
        }

        public async Task<Atlas.Core.Entities.Category?> FindByNameAsync(string categoryName)
        {
            return await _context.Categories
                                 .FirstOrDefaultAsync(c => c.CategoryName == categoryName);
        }

        public async Task<bool> AddAsync(Atlas.Core.Entities.Category category)
        {
            try 
            {
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Atlas.Core.Entities.Category category)
        {
            try 
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return true;
            }
            catch 
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
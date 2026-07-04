using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly AtlasDBContext _context;

        public ImageRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Images image)
        {
            await _context.Images.AddAsync(image);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Images?> GetByIdAsync(int id)
        {
            return await _context.Images.FindAsync(id);
        }

        public async Task<IEnumerable<Images>> GetAllAsync()
        {
            return await _context.Images.ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null) return false;

            _context.Images.Remove(image);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Images?> GetByUrlAsync(string url)
        {
            return await _context.Images
                .FirstOrDefaultAsync(i => i.ImageUrl == url);
        }
    }
}
using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Repositories
{
    public class TaxRepository : ITaxRepository
    {
        private readonly AtlasDBContext _context;

        public TaxRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tax>> GetAllAsync()
        {
            return await _context.Taxes
                .Include(t => t.ProductTaxes)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Tax>> GetActiveAsync()
        {
            return await _context.Taxes
                .Where(t => t.IsActive)
                .Include(t => t.ProductTaxes)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Tax> GetByIdAsync(int id)
        {
            return await _context.Taxes
                .Include(t => t.ProductTaxes)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<bool> AddAsync(Tax tax)
        {
            await _context.Taxes.AddAsync(tax);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Tax tax)
        {
            _context.Taxes.Update(tax);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tax = await _context.Taxes.FindAsync(id);
            if (tax is null)
            {
                return false;
            }

            _context.Taxes.Remove(tax);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class PricelistRepository : IPricelistRepository
    {
        private readonly AtlasDBContext _context;

        public PricelistRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pricelist>> GetAllAsync()
        {
            return await _context.Pricelists
                .Include(p => p.Vendor)
                .Include(p => p.Currency)
                .Include(p => p.PricelistVariants)
                    .ThenInclude(pv => pv.Variant)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Pricelist?> GetByIdAsync(int id)
        {
            return await _context.Pricelists
                .Include(p => p.Vendor)
                .Include(p => p.Currency)
                .Include(p => p.PricelistVariants)
                    .ThenInclude(pv => pv.Variant)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Pricelist>> GetByVendorAsync(int vendorId)
        {
            return await _context.Pricelists
                .Where(p => p.VendorId == vendorId)
                .Include(p => p.Vendor)
                .Include(p => p.Currency)
                .Include(p => p.PricelistVariants)
                    .ThenInclude(pv => pv.Variant)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> AddAsync(Pricelist pricelist)
        {
            await _context.Pricelists.AddAsync(pricelist);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Pricelist pricelist)
        {
            _context.Pricelists.Update(pricelist);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pricelist = await _context.Pricelists.FindAsync(id);
            if (pricelist is null)
            {
                return false;
            }

            _context.Pricelists.Remove(pricelist);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
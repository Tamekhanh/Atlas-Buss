using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly AtlasDBContext _context;

        public WarehouseRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Warehouse>> GetAllAsync()
        {
            return await _context.Warehouses
                .Include(w => w.Address)
                .Include(w => w.Manager)
                // ĐÃ XÓA: .ThenInclude(m => m.Person)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Warehouse?> GetByIdAsync(int id)
        {
            return await _context.Warehouses
                .Include(w => w.Address)
                .Include(w => w.Manager)
                // ĐÃ XÓA: .ThenInclude(m => m.Person)
                .Include(w => w.InventoryStocks)
                    .ThenInclude(stock => stock.Variant) // BỔ SUNG: Kéo theo thông tin của Biến thể sản phẩm trong kho
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<bool> AddAsync(Warehouse warehouse)
        {
            await _context.Warehouses.AddAsync(warehouse);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Warehouse warehouse)
        {
            _context.Warehouses.Update(warehouse);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse is null)
            {
                return false;
            }

            _context.Warehouses.Remove(warehouse);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
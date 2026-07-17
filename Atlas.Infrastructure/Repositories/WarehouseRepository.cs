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
                .Where(w => !w.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Warehouse?> GetByIdAsync(int id)
        {
            // KHÔNG dùng AsNoTracking: controller cần entity đang tracked để cập nhật
            // cả Address lẫn các scalar fields và gọi UpdateAsync/save.
            return await _context.Warehouses
                .Include(w => w.Address)
                .Include(w => w.Manager)
                .Include(w => w.InventoryStocks)
                    .ThenInclude(stock => stock.Variant)
                .ThenInclude(variant => variant != null ? variant.Product : null)
                .FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<bool> AddAsync(Warehouse warehouse)
        {
            // Tạo Address trước (giống PartyRepository.CreateAsync) để AddressId được gán.
            if (warehouse.Address != null)
            {
                await _context.Addresses.AddAsync(warehouse.Address);
                await _context.SaveChangesAsync();
                warehouse.AddressId = warehouse.Address.Id;
            }

            await _context.Warehouses.AddAsync(warehouse);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Warehouse warehouse)
        {
            // Nếu Address là một entity mới (chưa được track), attach và đánh dấu Modified.
            if (warehouse.Address != null && warehouse.Address.Id == 0)
            {
                await _context.Addresses.AddAsync(warehouse.Address);
                await _context.SaveChangesAsync();
                warehouse.AddressId = warehouse.Address.Id;
            }
            else if (warehouse.Address != null)
            {
                _context.Addresses.Update(warehouse.Address);
            }

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

            // Soft delete để tránh phá dữ liệu InventoryStock / PO / SO đang dùng kho.
            warehouse.IsDeleted = true;
            _context.Warehouses.Update(warehouse);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

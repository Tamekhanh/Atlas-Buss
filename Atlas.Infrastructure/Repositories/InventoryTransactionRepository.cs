using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly AtlasDBContext _context;

        public InventoryTransactionRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetAllAsync()
        {
            return await _context.InventoryTransactions
                // Tùy thuộc vào Entity của bạn, nếu đã đổi sang Variant thì sửa t.Product thành t.Variant
                .Include(t => t.Variant) 
                .Include(t => t.Warehouse)
                .Include(t => t.Employee)
                // ĐÃ XÓA: .ThenInclude(e => e.Person)
                .AsNoTracking()
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<InventoryTransaction?> GetByIdAsync(long id)
        {
            return await _context.InventoryTransactions
                .Include(t => t.Variant)
                .Include(t => t.Warehouse)
                .Include(t => t.Employee)
                // ĐÃ XÓA: .ThenInclude(e => e.Person)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // LƯU Ý: Nếu Interface của bạn vẫn là GetByProductIdAsync, chúng ta sẽ Query qua Variant.ProductId
        public async Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(int productId)
        {
            return await _context.InventoryTransactions
                .Include(t => t.Variant)
                .Where(t => t.Variant != null && t.Variant.ProductId == productId)
                .Include(t => t.Warehouse)
                .Include(t => t.Employee)
                .AsNoTracking()
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByWarehouseIdAsync(int warehouseId)
        {
            return await _context.InventoryTransactions
                .Where(t => t.WarehouseId == warehouseId)
                .Include(t => t.Variant)
                .Include(t => t.Employee)
                .AsNoTracking()
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<bool> AddAsync(InventoryTransaction transaction)
        {
            await _context.InventoryTransactions.AddAsync(transaction);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
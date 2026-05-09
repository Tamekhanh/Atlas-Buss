using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                .Include(t => t.Product)
                .Include(t => t.Warehouse)
                .Include(t => t.Employee)
                .ThenInclude(e => e.Person)
                .AsNoTracking()
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<InventoryTransaction> GetByIdAsync(long id)
        {
            return await _context.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.Warehouse)
                .Include(t => t.Employee)
                .ThenInclude(e => e.Person)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByProductIdAsync(int productId)
        {
            return await _context.InventoryTransactions
                .Where(t => t.ProductId == productId)
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
                .Include(t => t.Product)
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

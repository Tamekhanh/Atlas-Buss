using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class PurchaseOrderBillRepository : IPurchaseOrderBillRepository
    {
        private readonly AtlasDBContext _context;

        public PurchaseOrderBillRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(PurchaseOrderBill bill)
        {
            await _context.PurchaseOrderBills.AddAsync(bill);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<PurchaseOrderBill>> GetByOrderIdAsync(int orderId)
        {
            return await _context.PurchaseOrderBills
                .AsNoTracking()
                .Where(b => b.OrderId == orderId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<PurchaseOrderBill?> GetByIdAsync(int id)
        {
            return await _context.PurchaseOrderBills.FindAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var bill = await _context.PurchaseOrderBills.FindAsync(id);
            if (bill == null) return false;

            _context.PurchaseOrderBills.Remove(bill);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

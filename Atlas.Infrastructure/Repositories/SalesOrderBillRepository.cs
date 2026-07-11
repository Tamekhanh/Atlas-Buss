using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class SalesOrderBillRepository : ISalesOrderBillRepository
    {
        private readonly AtlasDBContext _context;

        public SalesOrderBillRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(SalesOrderBill bill)
        {
            await _context.SalesOrderBills.AddAsync(bill);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<SalesOrderBill>> GetByOrderIdAsync(int orderId)
        {
            return await _context.SalesOrderBills
                .AsNoTracking()
                .Where(b => b.OrderId == orderId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<SalesOrderBill?> GetByIdAsync(int id)
        {
            return await _context.SalesOrderBills.FindAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var bill = await _context.SalesOrderBills.FindAsync(id);
            if (bill == null) return false;

            _context.SalesOrderBills.Remove(bill);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

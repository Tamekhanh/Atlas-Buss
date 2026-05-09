using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Repositories
{
    public class SalesOrderRepository : ISalesOrderRepository
    {
        private readonly AtlasDBContext _context;

        public SalesOrderRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesOrder>> GetAllAsync()
        {
            return await _context.SalesOrders
                .Include(o => o.Employee)
                .ThenInclude(e => e.Person)
                .Include(o => o.CustomerCompany)
                .ThenInclude(cc => cc.Company)
                .Include(o => o.CustomerPerson)
                .ThenInclude(cp => cp.Person)
                .Include(o => o.SalesOrderDetails)
                .ThenInclude(d => d.Product)
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<SalesOrder> GetByIdAsync(int id)
        {
            return await _context.SalesOrders
                .Include(o => o.Employee)
                .ThenInclude(e => e.Person)
                .Include(o => o.CustomerCompany)
                .ThenInclude(cc => cc.Company)
                .Include(o => o.CustomerPerson)
                .ThenInclude(cp => cp.Person)
                .Include(o => o.SalesOrderDetails)
                .ThenInclude(d => d.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<SalesOrder> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.SalesOrders
                .Include(o => o.Employee)
                .ThenInclude(e => e.Person)
                .Include(o => o.CustomerCompany)
                .ThenInclude(cc => cc.Company)
                .Include(o => o.CustomerPerson)
                .ThenInclude(cp => cp.Person)
                .Include(o => o.SalesOrderDetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<bool> AddAsync(SalesOrder order)
        {
            await _context.SalesOrders.AddAsync(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(SalesOrder order)
        {
            _context.SalesOrders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _context.SalesOrders.FindAsync(id);
            if (order is null)
            {
                return false;
            }

            _context.SalesOrders.Remove(order);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

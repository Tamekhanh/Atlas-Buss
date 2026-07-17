using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                .Include(o => o.Employee) // Đã bỏ ThenInclude(Person)
                .Include(o => o.Customer) // Trỏ về Party
                .Include(o => o.SalesOrderDetails)
                    .ThenInclude(d => d.Variant) // Trỏ về Variant thay vì Product
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<SalesOrder?> GetByIdAsync(int id)
        {
            return await _context.SalesOrders
                .Include(o => o.Employee)
                .Include(o => o.Customer)
                .Include(o => o.SalesOrderDetails)
                    .ThenInclude(d => d.Variant)
                        .ThenInclude(v => v.Product)
                .Include(o => o.SalesOrderDetails)
                    .ThenInclude(d => d.Warehouse)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<SalesOrder?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.SalesOrders
                .Include(o => o.Employee)
                .Include(o => o.Customer)
                .Include(o => o.SalesOrderDetails)
                    .ThenInclude(d => d.Variant)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<IEnumerable<string>> GetAllNumbersAsync()
        {
            // Chỉ lấy cột số SO (bỏ qua bản ghi đã xóa mềm) để tối ưu.
            return await _context.SalesOrders
                .Where(o => !o.IsDeleted)
                .Select(o => o.OrderNumber)
                .AsNoTracking()
                .ToListAsync();
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
using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AtlasDBContext _context;

        public PurchaseOrderRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllAsync()
        {
            return await _context.PurchaseOrders
                .Include(o => o.Employee) 
                // SỬA TẠI ĐÂY: o.Parties -> o.Vendor
                .Include(o => o.Vendor)
                .Include(o => o.PurchaseOrderDetails)
                    .ThenInclude(d => d.Variant)
                        .ThenInclude(v => v.Product)
                .Include(o => o.PurchaseOrderDetails)
                    .ThenInclude(d => d.Variant)
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder?> GetByIdAsync(int id)
        {
            return await _context.PurchaseOrders
                .Include(o => o.Employee)
                .Include(o => o.Vendor)
                .Include(o => o.PurchaseOrderDetails)
                    .ThenInclude(d => d.Variant)
                        .ThenInclude(v => v.Product)
                .Include(o => o.PurchaseOrderDetails)
                    .ThenInclude(d => d.Variant)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<PurchaseOrder?> GetByPONumberAsync(string poNumber)
        {
            return await _context.PurchaseOrders
                .Include(o => o.Employee)
                .Include(o => o.Vendor)
                .Include(o => o.PurchaseOrderDetails)
                    .ThenInclude(d => d.Variant)
                        .ThenInclude(v => v.Product)
                .Include(o => o.PurchaseOrderDetails)
                    .ThenInclude(d => d.Variant)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.PONumber == poNumber);
        }

        public async Task<bool> AddAsync(PurchaseOrder order)
        {
            await _context.PurchaseOrders.AddAsync(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(PurchaseOrder order)
        {
            _context.PurchaseOrders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _context.PurchaseOrders.FindAsync(id);
            if (order is null)
            {
                return false;
            }

            _context.PurchaseOrders.Remove(order);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
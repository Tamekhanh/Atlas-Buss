using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                .ThenInclude(e => e.Person)
                .Include(o => o.VendorCompany)
                .ThenInclude(vc => vc.Company)
                .Include(o => o.VendorPerson)
                .ThenInclude(vp => vp.Person)
                .Include(o => o.PurchaseOrderDetails)
                .ThenInclude(d => d.Product)
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder> GetByIdAsync(int id)
        {
            return await _context.PurchaseOrders
                .Include(o => o.Employee)
                .ThenInclude(e => e.Person)
                .Include(o => o.VendorCompany)
                .ThenInclude(vc => vc.Company)
                .Include(o => o.VendorPerson)
                .ThenInclude(vp => vp.Person)
                .Include(o => o.PurchaseOrderDetails)
                .ThenInclude(d => d.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<PurchaseOrder> GetByPONumberAsync(string poNumber)
        {
            return await _context.PurchaseOrders
                .Include(o => o.Employee)
                .ThenInclude(e => e.Person)
                .Include(o => o.VendorCompany)
                .ThenInclude(vc => vc.Company)
                .Include(o => o.VendorPerson)
                .ThenInclude(vp => vp.Person)
                .Include(o => o.PurchaseOrderDetails)
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

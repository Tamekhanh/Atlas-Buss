using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class MyCompanyInfoRepository : IMyCompanyInfoRepository
    {
        private readonly AtlasDBContext _context;

        public MyCompanyInfoRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<MyCompanyInfo?> GetAsync()
        {
            return await _context.MyCompanyInfo
                .AsNoTracking()
                .Include(c => c.Logo)
                .OrderBy(c => c.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddAsync(MyCompanyInfo info)
        {
            await _context.MyCompanyInfo.AddAsync(info);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(MyCompanyInfo info)
        {
            var existing = await _context.MyCompanyInfo
                .OrderBy(c => c.Id)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                await _context.MyCompanyInfo.AddAsync(info);
            }
            else
            {
                existing.CompanyName = info.CompanyName;
                existing.Address = info.Address;
                existing.PhoneNumber = info.PhoneNumber;
                existing.Email = info.Email;
                existing.TaxId = info.TaxId;
                existing.LogoId = info.LogoId;
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}

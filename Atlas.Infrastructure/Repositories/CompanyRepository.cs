using System;
using System.Collections.Generic;
using System.Text;
using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AtlasDBContext _context;

        public CompanyRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Companies>> GetAllCompaniesAsync()
        {
            return await _context.Companies
                .Include(company => company.Address)
                .Include(company => company.Contact)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Companies?> GetCompanyByIdAsync(int id)
        {
            return await _context.Companies
                .Include(company => company.Address)
                .Include(company => company.Contact)
                .AsNoTracking()
                .FirstOrDefaultAsync(company => company.Id == id);
        }

        public async Task<bool> AddCompanyAsync(Companies company)
        {
            await _context.Companies.AddAsync(company);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCompanyAsync(Companies company)
        {
            _context.Companies.Update(company);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCompanyAsync(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company is null)
            {
                return false;
            }

            _context.Companies.Remove(company);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

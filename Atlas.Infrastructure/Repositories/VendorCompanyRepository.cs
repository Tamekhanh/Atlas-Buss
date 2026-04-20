using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Repositories
{
    public class VendorCompanyRepository : IVendorCompanyRepository
    {
        private readonly AtlasDBContext _context;

        public VendorCompanyRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VendorCompany>> GetAllVendorCompaniesAsync()
        {
            return await _context.VendorCompanies
                .Include(vendor => vendor.Company)
                    .ThenInclude(company => company!.Contact)
                .Include(vendor => vendor.Company)
                    .ThenInclude(company => company!.Address)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> CreateVendorCompanyAsync(CompanyRegistrationRequest request)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var contact = new Contacts
                    {
                        Phone = request.Phone.Trim(),
                        Email = request.Email.Trim()
                    };
                    await _context.Contacts.AddAsync(contact);
                    await _context.SaveChangesAsync();

                    var address = new Addresses
                    {
                        AddressType = string.IsNullOrWhiteSpace(request.AddressType) ? "HeadOffice" : request.AddressType.Trim(),
                        Street = request.Street.Trim(),
                        City = request.City.Trim(),
                        State = request.State.Trim(),
                        Country = request.Country.Trim()
                    };
                    await _context.Addresses.AddAsync(address);
                    await _context.SaveChangesAsync();

                    var company = new Companies
                    {
                        CompanyName = request.CompanyName.Trim(),
                        TaxId = request.TaxId.Trim(),
                        AddressId = address.Id,
                        ContactId = contact.Id
                    };
                    await _context.Companies.AddAsync(company);
                    await _context.SaveChangesAsync();

                    var vendorCompany = new VendorCompany
                    {
                        CompanyId = company.Id
                    };
                    await _context.VendorCompanies.AddAsync(vendorCompany);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}

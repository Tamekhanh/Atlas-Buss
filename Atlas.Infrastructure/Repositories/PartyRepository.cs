using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class PartyRepository : IPartyRepository
    {
        private readonly AtlasDBContext _context;

        public PartyRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Party>> GetAllAsync()
        {
            return await _context.Parties
                .Include(p => p.Address)
                .Include(p => p.Contact)
                .Where(p => !p.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<bool> AddAsync(Party party)
        {
            await _context.Parties.AddAsync(party);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Party>> GetCustomersAsync()
        {
            return await _context.Parties
                .Where(p => p.IsCustomer && !p.IsDeleted)
                .Include(p => p.Address)
                .Include(p => p.Contact)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Party>> GetVendorsAsync()
        {
            return await _context.Parties
                .Where(p => p.IsVendor && !p.IsDeleted)
                .Include(p => p.Address)
                .Include(p => p.Contact)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Party?> GetByIdAsync(int id)
        {
            return await _context.Parties
                .Include(p => p.Address)
                .Include(p => p.Contact)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<bool> GetDeletedStatusAsync(int id)
        {
            var party = await _context.Parties.FindAsync(id);
            return party?.IsDeleted ?? false;
        }


        // Hàm Create duy nhất thay thế cho 4 hàm Create cũ
        public async Task<bool> CreateAsync(PartyRegistrationRequest request)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // 1. Lưu thông tin liên lạc (Contact)
                    var contact = new Contacts
                    {
                        Phone = request.Contact.Phone.Trim(),
                        Email = string.IsNullOrWhiteSpace(request.Contact.Email) ? null : request.Contact.Email.Trim()
                    };
                    await _context.Contacts.AddAsync(contact);
                    await _context.SaveChangesAsync();

                    // 2. Lưu thông tin địa chỉ (Address)
                    var address = new Addresses
                    {
                        AddressType = string.IsNullOrWhiteSpace(request.Address.AddressType) ? "Office" : request.Address.AddressType.Trim(),
                        Street = request.Address.Street.Trim(),
                        City = request.Address.City.Trim(),
                        State = request.Address.State.Trim(),
                        Country = request.Address.Country.Trim()
                    };
                    await _context.Addresses.AddAsync(address);
                    await _context.SaveChangesAsync();

                    // 3. Lưu thông tin đối tác chung (Party)
                    var party = new Party
                    {
                        PartyType = request.PartyType.Trim(), // Nhận giá trị "Person" hoặc "Company"
                        DisplayName = request.DisplayName.Trim(),
                        FirstName = string.IsNullOrWhiteSpace(request.FirstName) ? null : request.FirstName.Trim(),
                        LastName = string.IsNullOrWhiteSpace(request.LastName) ? null : request.LastName.Trim(),
                        DoB = request.DoB,
                        TaxId = string.IsNullOrWhiteSpace(request.TaxId) ? null : request.TaxId.Trim(),
                        AddressId = address.Id,
                        ContactId = contact.Id,
                        IsCustomer = request.IsCustomer, // Xác định có phải khách hàng không
                        IsVendor = request.IsVendor      // Xác định có phải nhà cung cấp không
                    };

                    await _context.Parties.AddAsync(party);
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

        public async Task<bool> UpdateAsync(Party party)
        {
            _context.Parties.Update(party);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var party = await _context.Parties.FindAsync(id);
            if (party == null) return false;

            party.IsDeleted = true; // Soft delete
            _context.Parties.Update(party);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
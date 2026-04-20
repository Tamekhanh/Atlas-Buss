using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Repositories
{
    public class VendorPersonRepository : IVendorPersonRepository
    {
        private readonly AtlasDBContext _context;

        public VendorPersonRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VendorPerson>> GetAllVendorPersonsAsync()
        {
            return await _context.VendorPersons
                .Include(vendorPerson => vendorPerson.Person)
                    .ThenInclude(person => person!.Address)
                .Include(vendorPerson => vendorPerson.Person)
                    .ThenInclude(person => person!.Contact)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> CreateVendorPersonAsync(PersonRegistrationRequest request)
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
                        AddressType = string.IsNullOrWhiteSpace(request.AddressType) ? "Home" : request.AddressType.Trim(),
                        Street = request.Street.Trim(),
                        City = request.City.Trim(),
                        State = request.State.Trim(),
                        Country = request.Country.Trim()
                    };
                    await _context.Addresses.AddAsync(address);
                    await _context.SaveChangesAsync();

                    var person = new Person
                    {
                        FirstName = request.FirstName.Trim(),
                        LastName = request.LastName.Trim(),
                        DoB = request.DoB.Date,
                        AddressId = address.Id,
                        ContactId = contact.Id
                    };
                    await _context.Persons.AddAsync(person);
                    await _context.SaveChangesAsync();

                    var vendorPerson = new VendorPerson
                    {
                        PersonId = person.Id,
                        TaxId = string.IsNullOrWhiteSpace(request.TaxId) ? null : request.TaxId.Trim()
                    };
                    await _context.VendorPersons.AddAsync(vendorPerson);
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

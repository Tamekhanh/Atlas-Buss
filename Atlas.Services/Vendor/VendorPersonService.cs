using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Core.Models;

namespace Atlas.Services.Vendor
{
    public class VendorPersonService : IVendorPersonService
    {
        private readonly IVendorPersonRepository _vendorPersonRepository;

        public VendorPersonService(IVendorPersonRepository vendorPersonRepository)
        {
            _vendorPersonRepository = vendorPersonRepository;
        }

        public async Task<IEnumerable<VendorPerson>> GetAllAsync()
        {
            return await _vendorPersonRepository.GetAllVendorPersonsAsync();
        }

        public async Task<bool> CreateAsync(PersonRegistrationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName)
                || string.IsNullOrWhiteSpace(request.LastName))
            {
                return false;
            }

            return await _vendorPersonRepository.CreateVendorPersonAsync(request);
        }
    }
}

using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Core.Models;

namespace Atlas.Services.Vendor
{
    public class VendorCompanyService : IVendorCompanyService
    {
        private readonly IVendorCompanyRepository _vendorCompanyRepository;

        public VendorCompanyService(IVendorCompanyRepository vendorCompanyRepository)
        {
            _vendorCompanyRepository = vendorCompanyRepository;
        }

        public async Task<IEnumerable<VendorCompany>> GetAllAsync()
        {
            return await _vendorCompanyRepository.GetAllVendorCompaniesAsync();
        }

        public async Task<bool> CreateAsync(CompanyRegistrationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CompanyName) || string.IsNullOrWhiteSpace(request.TaxId))
            {
                return false;
            }

            return await _vendorCompanyRepository.CreateVendorCompanyAsync(request);
        }
    }
}

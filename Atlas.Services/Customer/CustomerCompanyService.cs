using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Core.Models;

namespace Atlas.Services.Customer
{
    public class CustomerCompanyService : ICustomerCompanyService
    {
        private readonly ICustomerCompanyRepository _customerCompanyRepository;

        public CustomerCompanyService(ICustomerCompanyRepository customerCompanyRepository)
        {
            _customerCompanyRepository = customerCompanyRepository;
        }

        public async Task<IEnumerable<CustomerCompany>> GetAllAsync()
        {
            return await _customerCompanyRepository.GetAllCustomerCompaniesAsync();
        }

        public async Task<bool> CreateAsync(CompanyRegistrationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CompanyName) || string.IsNullOrWhiteSpace(request.TaxId))
            {
                return false;
            }

            return await _customerCompanyRepository.CreateCustomerCompanyAsync(request);
        }
    }
}

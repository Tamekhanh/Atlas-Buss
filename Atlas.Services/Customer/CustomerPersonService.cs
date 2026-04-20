using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Core.Models;

namespace Atlas.Services.Customer
{
    public class CustomerPersonService : ICustomerPersonService
    {
        private readonly ICustomerPersonRepository _customerPersonRepository;

        public CustomerPersonService(ICustomerPersonRepository customerPersonRepository)
        {
            _customerPersonRepository = customerPersonRepository;
        }

        public async Task<IEnumerable<CustomerPerson>> GetAllAsync()
        {
            return await _customerPersonRepository.GetAllCustomerPersonsAsync();
        }

        public async Task<bool> CreateAsync(PersonRegistrationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName)
                || string.IsNullOrWhiteSpace(request.LastName))
            {
                return false;
            }

            return await _customerPersonRepository.CreateCustomerPersonAsync(request);
        }
    }
}

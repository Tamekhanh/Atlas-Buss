using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Core.Entities;
using Atlas.Core.Models; // Chứa PartyRegistrationRequest

namespace Atlas.Core.Interfaces
{
    public interface IPartyService
    {
        Task<IEnumerable<Party>> GetAllAsync();
        Task<IEnumerable<Party>> GetCustomersAsync();
        Task<IEnumerable<Party>> GetVendorsAsync();
        Task<Party?> GetByIdAsync(int id);
        
        // Sử dụng một DTO duy nhất thay thế cho CompanyRegistrationRequest & PersonRegistrationRequest
        Task<bool> CreateAsync(PartyRegistrationRequest request);
        
        Task<bool> UpdateAsync(Party party); 
        Task<bool> DeleteAsync(int id);
        Task<bool> GetDeletedStatusAsync(int id);
    }
}
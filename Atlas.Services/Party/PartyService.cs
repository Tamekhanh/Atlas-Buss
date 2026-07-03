using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Services
{
    public class PartyService : IPartyService
    {
        private readonly IPartyRepository _partyRepository;

        public PartyService(IPartyRepository partyRepository)
        {
            _partyRepository = partyRepository;
        }

        public async Task<IEnumerable<Party>> GetAllAsync()
        {
            return await _partyRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Party>> GetCustomersAsync()
        {
            return await _partyRepository.GetCustomersAsync();
        }

        public async Task<IEnumerable<Party>> GetVendorsAsync()
        {
            return await _partyRepository.GetVendorsAsync();
        }

        public async Task<Party?> GetByIdAsync(int id)
        {
            return await _partyRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateAsync(PartyRegistrationRequest request)
        {
            // 1. Kiểm tra loại đối tác có hợp lệ không
            if (string.IsNullOrWhiteSpace(request.PartyType) || 
                (request.PartyType != "Company" && request.PartyType != "Person"))
            {
                return false;
            }

            // 2. Logic kiểm tra dành riêng cho Công ty
            if (request.PartyType == "Company")
            {
                if (string.IsNullOrWhiteSpace(request.DisplayName) || string.IsNullOrWhiteSpace(request.TaxId))
                {
                    return false;
                }
            }

            // 3. Logic kiểm tra dành riêng cho Cá nhân
            if (request.PartyType == "Person")
            {
                if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                {
                    return false;
                }
                
                // Tự động ghép FullName cho cá nhân nếu DisplayName chưa được truyền
                if (string.IsNullOrWhiteSpace(request.DisplayName))
                {
                    request.DisplayName = $"{request.FirstName.Trim()} {request.LastName.Trim()}";
                }
            }

            // 4. Kiểm tra xem ít nhất phải có một vai trò
            if (!request.IsCustomer && !request.IsVendor)
            {
                return false; 
            }

            return await _partyRepository.CreateAsync(request);
        }

        public async Task<bool> UpdateAsync(Party party)
        {
            if (party == null || party.Id <= 0)
            {
                return false;
            }

            return await _partyRepository.UpdateAsync(party);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            return await _partyRepository.DeleteAsync(id);
        }

        public async Task<bool> GetDeletedStatusAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            return await _partyRepository.GetDeletedStatusAsync(id);
        }
    }
}
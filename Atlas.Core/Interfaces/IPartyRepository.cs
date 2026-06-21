using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Core.Entities;
using Atlas.Core.Models; // Bắt buộc phải có dòng này để nhận diện PartyRegistrationRequest

namespace Atlas.Core.Interfaces
{
    public interface IPartyRepository
    {
        Task<IEnumerable<Party>> GetAllAsync();
        Task<IEnumerable<Party>> GetCustomersAsync();
        Task<IEnumerable<Party>> GetVendorsAsync();
        Task<Party?> GetByIdAsync(int id);
        
        // Khai báo hàm CreateAsync xử lý DTO
        Task<bool> CreateAsync(PartyRegistrationRequest request);
        
        // Khai báo hàm AddAsync cơ bản (đã thêm ở bước trước)
        Task<bool> AddAsync(Party party);
        
        Task<bool> UpdateAsync(Party party);
        Task<bool> DeleteAsync(int id);
    }
}
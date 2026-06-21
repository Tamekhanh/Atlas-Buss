using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IPricelistRepository
    {
        Task<IEnumerable<Pricelist>> GetAllAsync();
        
        // SỬA: Thêm dấu ? để cho phép trả về null, triệt tiêu cảnh báo CS8613
        Task<Pricelist?> GetByIdAsync(int id); 
        
        // SỬA: Gộp GetByVendorCompany và GetByVendorPerson thành 1 hàm duy nhất trỏ tới Party
        Task<IEnumerable<Pricelist>> GetByVendorAsync(int vendorId); 
        
        Task<bool> AddAsync(Pricelist pricelist);
        Task<bool> UpdateAsync(Pricelist pricelist);
        Task<bool> DeleteAsync(int id);
    }
}
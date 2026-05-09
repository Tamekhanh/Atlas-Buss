using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IPricelistRepository
    {
        Task<IEnumerable<Pricelist>> GetAllAsync();
        Task<Pricelist> GetByIdAsync(int id);
        Task<IEnumerable<Pricelist>> GetByVendorCompanyAsync(int vendorCompanyId);
        Task<IEnumerable<Pricelist>> GetByVendorPersonAsync(int vendorPersonId);
        Task<bool> AddAsync(Pricelist pricelist);
        Task<bool> UpdateAsync(Pricelist pricelist);
        Task<bool> DeleteAsync(int id);
    }
}

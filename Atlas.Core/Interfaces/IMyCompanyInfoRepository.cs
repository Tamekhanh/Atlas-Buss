using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IMyCompanyInfoRepository
    {
        Task<MyCompanyInfo?> GetAsync();
        Task<bool> AddAsync(MyCompanyInfo info);
        Task<bool> UpdateAsync(MyCompanyInfo info);
    }
}

using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    public interface IAccountRepository
    {
        Task<IEnumerable<EmployeeAccount>> GetAllAccountsAsync();
        Task<EmployeeAccount?> GetAccountByIdAsync(int employeeId);
        Task<EmployeeAccount?> FindAccountByUsernameAsync(string username);
        Task<bool> AddAccountAsync(EmployeeAccount account);
        Task<bool> UpdateAccountAsync(EmployeeAccount account);
        Task<bool> DeleteAccountAsync(int employeeId);
    }
}
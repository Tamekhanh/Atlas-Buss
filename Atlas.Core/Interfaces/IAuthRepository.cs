using Atlas.Core.Entities;
using Atlas.Core.Models;

namespace Atlas.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<AuthAccountSnapshot?> GetByUsernameAsync(string username);
        Task<List<string>> GetPermissionKeysByRoleIdAsync(int roleId);
        Task<bool> IsActiveByUsernameAsync(string username);
        Task<bool> UpdateLastLoginAsync(int employeeId, DateTime loginTimeUtc);
        Task<bool> UsernameExistsAsync(string username);
        Task<Employee?> GetEmployeeByNumberAsync(string employeeNumber);
        Task<bool> AddAccountAsync(EmployeeAccount account);
    }
}

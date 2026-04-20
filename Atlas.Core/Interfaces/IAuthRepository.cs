using Atlas.Core.Entities;

namespace Atlas.Core.Interfaces
{
    public interface IAuthRepository
    {
        Task<EmployeeAccount?> GetByUsernameAsync(string username);
        Task<bool> UpdateLastLoginAsync(int employeeId, DateTime loginTimeUtc);
        Task<bool> UsernameExistsAsync(string username);
        Task<Employee?> GetEmployeeByNumberAsync(string employeeNumber);
        Task<bool> AddAccountAsync(EmployeeAccount account);
    }
}

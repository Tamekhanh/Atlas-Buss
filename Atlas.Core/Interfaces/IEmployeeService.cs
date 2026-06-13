using Atlas.Core.Entities;

namespace Atlas.Core.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync(int pageNumber, int pageSize);
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm = null, string? EmployeeNumber = null);
    }
}
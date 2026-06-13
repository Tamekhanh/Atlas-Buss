using System.Collections.Generic;
using System.Threading.Tasks;
// THÊM DÒNG NÀY: Để trình biên dịch tìm thấy lớp Employee
using Atlas.Core.Entities; 

namespace Atlas.Core.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync(int pageNumber, int pageSize);
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee> CreateEmployeeAsync(Employee employee); 
        Task<bool> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm = null, string? employeeNumber = null);
    }
}
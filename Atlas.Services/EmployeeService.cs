using Atlas.Core.Entities;
using Atlas.Core.Interfaces;

namespace Atlas.Services.HRM
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        public async Task<bool> CreateEmployeeAsync(Employee employee)
        {
            if (string.IsNullOrWhiteSpace(employee.EmployeeNumber))
            {
                return false;
            }

            return await _employeeRepository.AddAsync(employee);
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            if (employee.Id <= 0 || string.IsNullOrWhiteSpace(employee.EmployeeNumber))
            {
                return false;
            }

            return await _employeeRepository.UpdateAsync(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            return await _employeeRepository.DeleteAsync(id);
        }
    }
}
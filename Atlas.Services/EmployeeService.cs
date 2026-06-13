using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Services.HRM
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(int pageNumber, int pageSize)
        {
            return await _employeeRepository.GetAllAsync(pageNumber, pageSize);
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _employeeRepository.GetByIdAsync(id);
        }

        // SỬA TẠI ĐÂY: Thay Task<bool> thành Task<Employee>
        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            if (string.IsNullOrWhiteSpace(employee.EmployeeNumber))
            {
                throw new ArgumentException("Employee number is required.");
            }

            var result = await _employeeRepository.CreateEmployeeAsync(employee);

            return result;
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

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm = null, string? EmployeeNumber = null)
        {
            return await _employeeRepository.SearchEmployeesAsync(searchTerm, EmployeeNumber);
        }
    }
}
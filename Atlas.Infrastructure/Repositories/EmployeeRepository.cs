using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AtlasDBContext _context;

        public EmployeeRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync(int pageNumber, int pageSize)
        {
            return await _context.Employees
                // Trỏ thẳng tới Address thay vì qua Person
                .Include(employee => employee.Address)
                .Include(employee => employee.Account)
                    .ThenInclude(account => account!.Role)
                        .ThenInclude(role => role!.RolePermissions)
                            .ThenInclude(rolePermission => rolePermission.Permission)
                .AsNoTracking()
                .OrderBy(employee => employee.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                // Trỏ thẳng tới Address và Contact thay vì qua Person
                .Include(employee => employee.Address)
                .Include(employee => employee.Contact)
                .Include(employee => employee.Account)
                    .ThenInclude(account => account!.Role)
                        .ThenInclude(role => role!.RolePermissions)
                            .ThenInclude(rolePermission => rolePermission.Permission)
                .FirstOrDefaultAsync(employee => employee.Id == id);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee is null)
            {
                return false;
            }

            _context.Employees.Remove(employee);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm = null, string? employeeNumber = null)
        {
            searchTerm = searchTerm?.Trim();
            employeeNumber = employeeNumber?.Trim();

            var query = _context.Employees
                // Trỏ thẳng tới Address
                .Include(employee => employee.Address)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Thay thế tìm kiếm FirstName/LastName qua Person bằng FullName trực tiếp
                query = query.Where(employee => employee.FullName.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(employeeNumber))
            {
                query = query.Where(employee => employee.EmployeeNumber.Contains(employeeNumber));
            }

            return await query
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
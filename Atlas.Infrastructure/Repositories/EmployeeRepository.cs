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
                .Include(employee => employee.Person)
                    .ThenInclude(person => person!.Address)
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
            // TỐI ƯU: Gộp các Include của Person lại với nhau để tránh lặp lại
            return await _context.Employees
                .Include(employee => employee.Person)
                    .ThenInclude(person => person!.Address)
                .Include(employee => employee.Person)
                    .ThenInclude(person => person!.Contact)
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
            // Sử dụng Update của EF Core
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

        // SỬA: Đổi tên tham số EmployeeNumber -> employeeNumber (quy chuẩn camelCase của C#)
        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchTerm = null, string? employeeNumber = null)
        {
            searchTerm = searchTerm?.Trim();
            employeeNumber = employeeNumber?.Trim();

            var query = _context.Employees
                .Include(employee => employee.Person)
                    .ThenInclude(person => person!.Address)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(employee =>
                    employee.Person!.FirstName.Contains(searchTerm) ||
                    employee.Person!.LastName.Contains(searchTerm));
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
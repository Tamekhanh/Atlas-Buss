using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AtlasDBContext _context;

        public AuthRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<AuthAccountSnapshot?> GetByUsernameAsync(string username)
        {
            return await _context.EmployeeAccounts
                .AsNoTracking()
                .Where(account => account.Username == username)
                .Select(account => new AuthAccountSnapshot
                {
                    EmployeeId = account.EmployeeId,
                    Username = account.Username,
                    PasswordHash = account.PasswordHash,
                    IsActive = account.IsActive,
                    RoleId = account.RoleId,
                    RoleName = account.Role != null ? account.Role.RoleName : null,
                    FirstName = account.Employee != null && account.Employee.Person != null ? account.Employee.Person.FirstName : string.Empty,
                    LastName = account.Employee != null && account.Employee.Person != null ? account.Employee.Person.LastName : string.Empty
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetPermissionKeysByRoleIdAsync(int roleId)
        {
            return await _context.RolePermissions
                .AsNoTracking()
                .Where(rolePermission => rolePermission.RoleId == roleId)
                .Select(rolePermission => rolePermission.Permission!.PermissionKey)
                .Where(permissionKey => !string.IsNullOrWhiteSpace(permissionKey))
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> IsActiveByUsernameAsync(string username)
        {
            return await _context.EmployeeAccounts
                .AsNoTracking()
                .AnyAsync(account => account.Username == username && account.IsActive);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.EmployeeAccounts
                .AnyAsync(account => account.Username == username);
        }

        public async Task<Employee?> GetEmployeeByNumberAsync(string employeeNumber)
        {
            return await _context.Employees
                .Include(employee => employee.Account)
                .Include(employee => employee.Person)
                .FirstOrDefaultAsync(employee => employee.EmployeeNumber == employeeNumber);
        }

        public async Task<bool> AddAccountAsync(EmployeeAccount account)
        {
            await _context.EmployeeAccounts.AddAsync(account);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateLastLoginAsync(int employeeId, DateTime loginTimeUtc)
        {
            var account = await _context.EmployeeAccounts
                .FirstOrDefaultAsync(employeeAccount => employeeAccount.EmployeeId == employeeId);

            if (account is null)
            {
                return false;
            }

            account.LastLogin = loginTimeUtc;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

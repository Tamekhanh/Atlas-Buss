using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
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

        public async Task<EmployeeAccount?> GetByUsernameAsync(string username)
        {
            return await _context.EmployeeAccounts
                .Include(account => account.Employee)
                    .ThenInclude(employee => employee!.Person)
                .FirstOrDefaultAsync(account => account.Username == username);
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

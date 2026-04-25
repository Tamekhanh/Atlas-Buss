using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly AtlasDBContext _context;

        public LogRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task AddLogAsync(Log log)
        {
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Log>> GetLogsByEmployeeIdAsync(int employeeId)
        {
            return await _context.Logs
                .Include(log => log.Employee)
                    .ThenInclude(employee => employee!.Account)
                .Include(log => log.Employee)
                    .ThenInclude(employee => employee!.Person)
                .Where(log => log.EmployeeId == employeeId)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetAllLogsAsync()
        {
            return await _context.Logs
                .Include(log => log.Employee)
                    .ThenInclude(employee => employee!.Account)
                .Include(log => log.Employee)
                    .ThenInclude(employee => employee!.Person)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }
    }
}
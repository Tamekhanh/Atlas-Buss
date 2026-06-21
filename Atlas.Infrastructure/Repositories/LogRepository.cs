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
                // Đã bỏ Include Person
                .Where(log => log.EmployeeId == employeeId)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetLogsByDateRangeAsync(DateTime? startDate = null, DateTime? endDate = null, int? employeeId = null, string? searchTerm = null)
        {
            var query = _context.Logs
                .Include(log => log.Employee)
                    .ThenInclude(employee => employee!.Account)
                // Đã bỏ Include Person
                .Where(log => (startDate == null || log.Timestamp >= startDate) && (endDate == null || log.Timestamp <= endDate));

            if (employeeId.HasValue)
            {
                query = query.Where(log => log.EmployeeId == employeeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                
                // Logic search đã được tinh gọn, dùng trực tiếp FullName
                query = query.Where(log =>
                    log.Action.Contains(term) ||
                    (log.Employee != null && (
                        (log.Employee.EmployeeNumber != null && log.Employee.EmployeeNumber.Contains(term)) ||
                        (log.Employee.Account != null && log.Employee.Account.Username != null && log.Employee.Account.Username.Contains(term)) ||
                        (log.Employee.FullName != null && log.Employee.FullName.Contains(term))
                    )));
            }

            return await query
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetAllLogsAsync()
        {
            return await _context.Logs
                .Include(log => log.Employee)
                    .ThenInclude(employee => employee!.Account)
                // Đã bỏ Include Person
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }
    }
}
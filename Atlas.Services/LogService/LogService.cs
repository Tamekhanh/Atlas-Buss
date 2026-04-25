using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Core.Models;

namespace Atlas.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task AddLogAsync(int? employeeId, string action)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                return;
            }

            var log = new Log
            {
                EmployeeId = employeeId,
                Action = action.Trim(),
                Timestamp = DateTime.Now
            };

            await _logRepository.AddLogAsync(log);
        }

        public async Task<IEnumerable<LogModel>> GetLogsByEmployeeIdAsync(int employeeId)
        {
            var logs = await _logRepository.GetLogsByEmployeeIdAsync(employeeId);
            return logs.Select(MapToModel);
        }

        public async Task<IEnumerable<LogModel>> GetAllLogsAsync()
        {
            var logs = await _logRepository.GetAllLogsAsync();
            return logs.Select(MapToModel);
        }

        private static LogModel MapToModel(Log log)
        {
            return new LogModel
            {
                Id = log.Id,
                EmployeeId = log.EmployeeId,
                Username = log.Employee?.Account?.Username,
                EmployeeNumber = log.Employee?.EmployeeNumber,
                EmployeeName = log.Employee?.Person is null
                    ? null
                    : $"{log.Employee.Person.FirstName} {log.Employee.Person.LastName}".Trim(),
                Action = log.Action,
                Timestamp = log.Timestamp
            };
        }
    }
}
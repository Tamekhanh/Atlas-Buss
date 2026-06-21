using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<LogModel>> GetLogsByDateRangeAsync(DateTime? startDate = null, DateTime? endDate = null, int? employeeId = null, string? searchTerm = null)
        {
            var logs = await _logRepository.GetLogsByDateRangeAsync(startDate, endDate, employeeId, searchTerm);
            return logs.Select(MapToModel);
        }

        private static LogModel MapToModel(Log log)
        {
            return new LogModel
            {
                Id = log.Id,
                EmployeeId = log.EmployeeId,
                
                // Lưu ý: Đảm bảo class Employee của bạn có Navigation Property "Account" (trỏ tới EmployeeAccounts)
                Username = log.Employee?.Account?.Username, 
                
                EmployeeNumber = log.Employee?.EmployeeNumber,
                
                // SỬA TẠI ĐÂY: Dùng trực tiếp FullName thay vì Person.FirstName/LastName
                EmployeeName = log.Employee?.FullName, 
                
                Action = log.Action,
                Timestamp = log.Timestamp
            };
        }
    }
}
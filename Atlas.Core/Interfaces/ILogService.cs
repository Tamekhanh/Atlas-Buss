using Atlas.Core.Models;

namespace Atlas.Core.Interfaces
{
    public interface ILogService
    {
        Task AddLogAsync(int? employeeId, string action);
        Task<IEnumerable<LogModel>> GetLogsByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<LogModel>> GetAllLogsAsync();
        Task<IEnumerable<LogModel>> GetLogsByDateRangeAsync(DateTime? startDate = null, DateTime? endDate = null, int? employeeId = null, string? searchTerm = null);
    }
}
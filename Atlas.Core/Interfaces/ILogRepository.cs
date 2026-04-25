using Atlas.Core.Entities;

namespace Atlas.Core.Interfaces
{ 
    public interface ILogRepository
    {
        Task AddLogAsync(Log log);
        Task<IEnumerable<Log>> GetLogsByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<Log>> GetAllLogsAsync();
    }
}
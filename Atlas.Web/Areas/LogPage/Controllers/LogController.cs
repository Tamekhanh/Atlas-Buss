using Atlas.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Web.Areas.LogPage.Controllers
{
    [Area("LogPage")]
    [Authorize]
    public class LogController : Controller
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate = null, DateTime? endDate = null, int? employeeId = null, string? searchTerm = null)
        {
            if (!startDate.HasValue && !endDate.HasValue && !employeeId.HasValue && string.IsNullOrWhiteSpace(searchTerm))
            {
                var logs = await _logService.GetAllLogsAsync();
                return View("~/Areas/LogPage/Views/LogPage/Index.cshtml", logs);
            }

            var filteredLogs = await _logService.GetLogsByDateRangeAsync(startDate, endDate, employeeId, searchTerm);
            return View("~/Areas/LogPage/Views/LogPage/Index.cshtml", filteredLogs);
        }

        [HttpPost]
        public async Task<IActionResult> FilterLogs(DateTime? startDate, DateTime? endDate, int? employeeId, string? searchTerm)
        {
            var logs = await _logService.GetLogsByDateRangeAsync(startDate, endDate, employeeId, searchTerm);
            return PartialView("~/Areas/LogPage/Views/LogPage/_LogTable.cshtml", logs);
        }
    }
}
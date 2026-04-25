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

        public async Task<IActionResult> Index()
        {
            var logs = await _logService.GetAllLogsAsync();
            return View("~/Areas/LogPage/Views/LogPage/Index.cshtml", logs);
        }
    }
}
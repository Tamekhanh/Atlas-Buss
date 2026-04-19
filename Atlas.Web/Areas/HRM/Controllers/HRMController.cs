using Atlas.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Web.Areas.HRM.Controllers
{
    [Area("HRM")]
    public class HRMController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public HRMController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View("~/Areas/HRM/Views/Home/Index.cshtml", employees);
        }
    }
}
using Atlas.Core.Interfaces;
using Atlas.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Web.Areas.HRM.Controllers
{
    [Area("HRM")]
    [Authorize]
    public class HRMController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public HRMController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var currentPage = page.GetValueOrDefault(1);
            var employees = await _employeeService.GetAllEmployeesAsync(currentPage, int.MaxValue);
            return View("~/Areas/HRM/Views/Home/Index.cshtml", employees);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee is null)
            {
                return NotFound();
            }

            return View("~/Areas/HRM/Views/Home/Detail.cshtml", employee);
        }

        public async Task<IActionResult> Search(string? searchTerm = null, string? employeeNumber = null)
        {
            var employees = await _employeeService.SearchEmployeesAsync(searchTerm, employeeNumber);
            return View("~/Areas/HRM/Views/Home/Index.cshtml", employees);
        }

        [Authorize(Policy = "EmployeeManage")]
        public IActionResult Create()
        {
            return View("~/Areas/HRM/Views/Home/Create.cshtml", new Employee());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "EmployeeManage")]
        public async Task<IActionResult> Create(Employee model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Areas/HRM/Views/Home/Create.cshtml", model);
            }

            try
            {
                // created bây giờ là kiểu Employee, không phải bool
                var created = await _employeeService.CreateEmployeeAsync(model);

                // Kiểm tra xem đối tượng có null hay không thay vì dùng !created
                if (created == null)
                {
                    ModelState.AddModelError(string.Empty, "Could not create employee.");
                    return View("~/Areas/HRM/Views/Home/Create.cshtml", model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Vì trong Service chúng ta có throw Exception khi fail, 
                // nên dùng try-catch để bắt lỗi và hiển thị ra màn hình
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return View("~/Areas/HRM/Views/Home/Create.cshtml", model);
            }
        }
    }
}
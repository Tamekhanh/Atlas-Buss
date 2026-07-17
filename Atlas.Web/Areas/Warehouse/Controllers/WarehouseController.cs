using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Web.Areas.Warehouse.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Atlas.Web.Areas.Warehouse.Controllers
{
    [Area("Warehouse")]
    [Authorize(Policy = "WarehouseView")]
    public class WarehouseController : Controller
    {
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogService _logService;

        public WarehouseController(
            IWarehouseRepository warehouseRepository,
            IEmployeeRepository employeeRepository,
            ILogService logService)
        {
            _warehouseRepository = warehouseRepository;
            _employeeRepository = employeeRepository;
            _logService = logService;
        }

        // GET: /Warehouse/Warehouse/Index
        public async Task<IActionResult> Index()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();

            var viewModel = warehouses
                .Where(w => !w.IsDeleted)
                .Select(w => new WarehouseListViewModel
                {
                    Id = w.Id,
                    WarehouseName = w.WarehouseName,
                    ManagerName = w.Manager?.FullName,
                    City = w.Address?.City,
                    Country = w.Address?.Country,
                    CreatedAt = w.CreatedAt,
                    InventoryItemCount = w.InventoryStocks?.Count ?? 0
                })
                .ToList();

            return View(viewModel);
        }

        // GET: /Warehouse/Warehouse/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            ViewData["ManagerName"] = warehouse.Manager?.FullName;
            ViewData["Address"] = warehouse.Address;

            var stockLines = (warehouse.InventoryStocks ?? new List<InventoryStock>())
                .Where(s => s.Variant != null)
                .Select(s => new WarehouseStockLineViewModel
                {
                    VariantId = s.VariantId,
                    VariantSKU = s.Variant?.SKU,
                    ProductName = s.Variant?.Product?.ProductName,
                    Quantity = s.Quantity,
                    ReservedQuantity = s.ReservedQuantity,
                    LastUpdated = s.LastUpdated
                })
                .ToList();

            ViewData["StockLines"] = stockLines;

            return View(warehouse);
        }

        // GET: /Warehouse/Warehouse/Create
        [Authorize(Policy = "WarehouseManage")]
        public async Task<IActionResult> Create()
        {
            await PopulateManagersAsync(null);
            return View(new WarehouseCreateViewModel());
        }

        // POST: /Warehouse/Warehouse/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "WarehouseManage")]
        public async Task<IActionResult> Create(WarehouseCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateManagersAsync(model.ManagerId);
                return View(model);
            }

            // EF Core sẽ tự tạo Addresses mới khi gán cho Address navigation.
            var warehouse = new Core.Entities.Warehouse
            {
                WarehouseName = model.WarehouseName,
                ManagerId = model.ManagerId,
                Address = new Addresses
                {
                    AddressType = string.IsNullOrWhiteSpace(model.AddressType) ? "Warehouse" : model.AddressType,
                    Street = model.Street,
                    City = model.City,
                    State = model.State,
                    Country = model.Country
                }
            };

            await _warehouseRepository.AddAsync(warehouse);

            await LogAsync($"Created new warehouse: {warehouse.WarehouseName}");

            TempData["SuccessMessage"] = "Warehouse created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Warehouse/Warehouse/Edit/5
        [Authorize(Policy = "WarehouseManage")]
        public async Task<IActionResult> Edit(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null || warehouse.IsDeleted)
            {
                return NotFound();
            }

            var model = new WarehouseCreateViewModel
            {
                Id = warehouse.Id,
                WarehouseName = warehouse.WarehouseName,
                ManagerId = warehouse.ManagerId,
                AddressType = warehouse.Address?.AddressType,
                Street = warehouse.Address?.Street,
                City = warehouse.Address?.City,
                State = warehouse.Address?.State,
                Country = warehouse.Address?.Country
            };

            await PopulateManagersAsync(model.ManagerId);
            return View(model);
        }

        // POST: /Warehouse/Warehouse/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "WarehouseManage")]
        public async Task<IActionResult> Edit(int id, WarehouseCreateViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await PopulateManagersAsync(model.ManagerId);
                return View(model);
            }

            // Lấy entity đang tracked để cập nhật cả Address lẫn các scalar.
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }

            warehouse.WarehouseName = model.WarehouseName;
            warehouse.ManagerId = model.ManagerId;

            if (warehouse.Address == null)
            {
                warehouse.Address = new Addresses();
            }
            warehouse.Address.AddressType = string.IsNullOrWhiteSpace(model.AddressType) ? "Warehouse" : model.AddressType;
            warehouse.Address.Street = model.Street;
            warehouse.Address.City = model.City;
            warehouse.Address.State = model.State;
            warehouse.Address.Country = model.Country;

            await _warehouseRepository.UpdateAsync(warehouse);

            await LogAsync($"Updated warehouse: {warehouse.WarehouseName} (ID: {warehouse.Id})");

            TempData["SuccessMessage"] = "Warehouse updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Warehouse/Warehouse/DeleteConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "WarehouseManage")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var warehouse = await _warehouseRepository.GetByIdAsync(id);
                if (warehouse == null)
                {
                    return NotFound();
                }

                // Soft delete thay vì xoá cứng để tránh phá các InventoryStock / PO / SO đang dùng kho.
                warehouse.IsDeleted = true;
                await _warehouseRepository.UpdateAsync(warehouse);

                await LogAsync($"Deleted warehouse: {warehouse.WarehouseName} (ID: {warehouse.Id})");

                TempData["SuccessMessage"] = "Warehouse deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["ErrorMessage"] = "Cannot delete this warehouse because it is linked to other data.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task PopulateManagersAsync(int? selectedManagerId)
        {
            // Lấy danh sách nhân viên để chọn người quản lý kho.
            var employees = await _employeeRepository.GetAllAsync(1, int.MaxValue);
            ViewBag.Managers = employees
                .Select(e => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = string.IsNullOrWhiteSpace(e.FullName) ? e.EmployeeNumber : $"{e.FullName} ({e.EmployeeNumber})",
                    Selected = selectedManagerId.HasValue && e.Id == selectedManagerId.Value
                })
                .ToList();
        }

        private async Task LogAsync(string message)
        {
            var employeeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(employeeIdValue, out var employeeId))
            {
                await _logService.AddLogAsync(employeeId, message);
            }
        }
    }
}

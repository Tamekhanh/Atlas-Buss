using System.Linq;
using System.Security.Claims;
using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Services;
using Atlas.Web.Areas.SaleOrder.Models;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Web.Areas.Sale.Controllers
{
	[Area("Sale")]
	[Route("Sale")]
	public class SaleOrderController : Controller
	{
		private readonly ISalesOrderService _salesOrderService;
		private readonly IPartyRepository _partyRepository;
		private readonly IProductRepository _productRepository;
		private readonly IWarehouseRepository _warehouseRepository;
		private readonly IStorageProvider _storageProvider;
		private readonly ISalesOrderBillRepository _soBillRepository;

		public SaleOrderController(
			ISalesOrderService salesOrderService,
			IPartyRepository partyRepository,
			IProductRepository productRepository,
			IWarehouseRepository warehouseRepository,
			IStorageProvider storageProvider,
			ISalesOrderBillRepository soBillRepository)
		{
			_salesOrderService = salesOrderService;
			_partyRepository = partyRepository;
			_productRepository = productRepository;
			_warehouseRepository = warehouseRepository;
			_storageProvider = storageProvider;
			_soBillRepository = soBillRepository;
		}

		[HttpGet]
		[Route("Index")]
		public async Task<IActionResult> Index()
		{
			var orders = await _salesOrderService.GetAllAsync();
			var model = orders.Select(order => new SaleOrderVM
			{
				Id = order.Id,
				OrderNumber = order.OrderNumber,
				OrderDate = order.OrderDate,
				CustomerId = order.CustomerId,
				CustomerName = order.Customer?.DisplayName ?? string.Empty,
				OrderStatusId = order.OrderStatusId,
				StatusName = order.OrderStatus?.StatusName ?? string.Empty,
				TotalAmount = order.TotalAmount
			}).ToList();

			return View(model);
		}

		[HttpGet]
		[Route("Details/{id}")]
		public async Task<IActionResult> Details(int id)
		{
			var order = await _salesOrderService.GetByIdAsync(id);
			if (order == null)
			{
				return NotFound();
			}

			var lines = order.SalesOrderDetails
				.OrderBy(d => d.Id)
				.Select(d =>
				{
					var lineTotal = (d.Quantity * d.UnitPrice) - d.Discount + d.TaxAmount;
					return new SaleOrderDetailLineVM
					{
						VariantId = d.VariantId,
						ProductName = d.Variant?.Product?.ProductName ?? string.Empty,
						VariantSKU = d.Variant?.SKU ?? string.Empty,
						WarehouseId = d.WarehouseId,
						WarehouseName = d.Warehouse?.WarehouseName ?? string.Empty,
						Quantity = d.Quantity,
						UnitPrice = d.UnitPrice,
						Discount = d.Discount,
						TaxAmount = d.TaxAmount,
						LineTotal = lineTotal
					};
				})
				.ToList();

			var model = new SaleOrderDetailPageVM
			{
				Id = order.Id,
				OrderNumber = order.OrderNumber,
				OrderDate = order.OrderDate,
				CustomerId = order.CustomerId,
				CustomerName = order.Customer?.DisplayName ?? string.Empty,
				EmployeeId = order.EmployeeId,
				EmployeeName = order.Employee?.FullName ?? string.Empty,
				OrderStatusId = order.OrderStatusId,
				StatusName = order.OrderStatus?.StatusName ?? string.Empty,
				CurrencyCode = order.Currency?.CurrencyCode ?? string.Empty,
				ExchangeRate = order.ExchangeRate,
				SubTotal = lines.Sum(l => l.Quantity * l.UnitPrice),
				TotalDiscount = lines.Sum(l => l.Discount),
				TotalTax = lines.Sum(l => l.TaxAmount),
				GrandTotal = order.TotalAmount,
				Lines = lines,
				Bills = (await _soBillRepository.GetByOrderIdAsync(order.Id))
					.Select(b => new SaleOrderBillVM
					{
						Id = b.Id,
						BillUrl = b.BillUrl,
						CreatedAt = b.CreatedAt
					})
					.ToList()
			};

			return View(model);
		}

		// Tải lên 1 file bill (PDF, ảnh scan, ...) đính kèm cho Sales Order
		[Route("UploadBill/{id}")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UploadBill(int id, Microsoft.AspNetCore.Http.IFormFile? billFile)
		{
			var order = await _salesOrderService.GetByIdAsync(id);
			if (order == null)
			{
				return NotFound();
			}

			if (billFile == null || billFile.Length == 0)
			{
				TempData["Error"] = "Vui lòng chọn file bill để tải lên.";
				return RedirectToAction(nameof(Details), new { id });
			}

			try
			{
				using var stream = billFile.OpenReadStream();
				var relativePath = await _storageProvider.SaveFileAsync(stream, "SaleBills", billFile.FileName);

				var bill = new SalesOrderBill
				{
					OrderId = order.Id,
					BillUrl = relativePath
				};
				await _soBillRepository.AddAsync(bill);
			}
			catch
			{
				TempData["Error"] = "Không thể lưu file bill. Vui lòng thử lại.";
			}

			return RedirectToAction(nameof(Details), new { id });
		}

		// Xóa 1 file bill đính kèm (chỉ xóa record; file vật lý trên ổ cứng được giữ lại)
		[Route("DeleteBill/{id}")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteBill(int id, int billId)
		{
			var order = await _salesOrderService.GetByIdAsync(id);
			if (order == null)
			{
				return NotFound();
			}

			var bill = await _soBillRepository.GetByIdAsync(billId);
			if (bill != null && bill.OrderId == order.Id)
			{
				await _soBillRepository.DeleteAsync(billId);
			}

			return RedirectToAction(nameof(Details), new { id });
		}

		[HttpGet]
		[Route("Create")]
		public async Task<IActionResult> Create()
		{
			var model = new SaleOrderCreateVM();
			await PopulateCreateLookupsAsync(model);
			return View(model);
		}

		[HttpPost]
		[Route("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(SaleOrderCreateVM model)
		{
			await PopulateCreateLookupsAsync(model);

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var employeeId = GetCurrentEmployeeId();
			if (employeeId <= 0)
			{
				employeeId = 1;
			}

			var salesOrder = new SalesOrder
			{
				OrderNumber = model.OrderNumber.Trim(),
				CustomerId = model.CustomerId,
				OrderDate = model.OrderDate,
				OrderStatusId = model.OrderStatusId,
				CurrencyId = model.CurrencyId,
				ExchangeRate = model.ExchangeRate,
				EmployeeId = employeeId,
				SalesOrderDetails = new List<SalesOrderDetail>()
			};

			if (model.OrderDetails != null && model.OrderDetails.Any())
			{
				foreach (var item in model.OrderDetails)
				{
					salesOrder.SalesOrderDetails.Add(new SalesOrderDetail
					{
						VariantId = item.VariantId,
						WarehouseId = item.WarehouseId,
						Quantity = item.Quantity,
						UnitPrice = item.UnitPrice,
						Discount = item.Discount,
						TaxAmount = item.TaxAmount
					});
				}
			}

			var result = await _salesOrderService.CreateAsync(salesOrder);
			if (result)
			{
				return RedirectToAction(nameof(Index));
			}

			ModelState.AddModelError(string.Empty, "Không thể lưu đơn bán hàng. Vui lòng kiểm tra lại dữ liệu.");
			return View(model);
		}

		private async Task PopulateCreateLookupsAsync(SaleOrderCreateVM model)
		{
			var customers = await _partyRepository.GetCustomersAsync();
			model.Customers = customers
				.Select(customer => new SaleOrderCustomerLookupVM
				{
					Id = customer.Id,
					DisplayName = customer.DisplayName
				})
				.ToList();

			var products = await _productRepository.GetAllWithVariantsAsync();
			model.Products = products
				.Select(product => new SaleOrderProductLookupVM
				{
					Id = product.Id,
					ProductName = product.ProductName,
					Variants = product.Variants
						.Select(variant => new SaleOrderVariantLookupVM
						{
							Id = variant.Id,
							ProductId = variant.ProductId,
							SKU = variant.SKU,
							VariantPrice = variant.VariantPrice
						})
						.ToList()
				})
				.ToList();

			var warehouses = await _warehouseRepository.GetAllAsync();
			model.Warehouses = warehouses
				.Where(warehouse => !warehouse.IsDeleted)
				.Select(warehouse => new SaleOrderWarehouseLookupVM
				{
					Id = warehouse.Id,
					WarehouseName = warehouse.WarehouseName
				})
				.ToList();
		}

		private int GetCurrentEmployeeId()
		{
			var employeeIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (int.TryParse(employeeIdClaim, out var employeeId))
			{
				return employeeId;
			}

			return 0;
		}
	}
}

using System.Linq;
using System.IO;
using System.Security.Claims;
using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Services;
using Atlas.Services.Report;
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
		private readonly IDocumentNumberService _documentNumberService;
		private readonly IBillTemplateRepository _billTemplateRepository;
		private readonly ISalesOrderReportService _reportService;
		private readonly ILogService _logService;

		public SaleOrderController(
			ISalesOrderService salesOrderService,
			IPartyRepository partyRepository,
			IProductRepository productRepository,
			IWarehouseRepository warehouseRepository,
			IStorageProvider storageProvider,
			ISalesOrderBillRepository soBillRepository,
			IDocumentNumberService documentNumberService,
			IBillTemplateRepository billTemplateRepository,
			ISalesOrderReportService reportService,
			ILogService logService)
		{
			_salesOrderService = salesOrderService;
			_partyRepository = partyRepository;
			_productRepository = productRepository;
			_warehouseRepository = warehouseRepository;
			_storageProvider = storageProvider;
			_soBillRepository = soBillRepository;
			_documentNumberService = documentNumberService;
			_billTemplateRepository = billTemplateRepository;
			_reportService = reportService;
			_logService = logService;
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
						BillSource = b.BillSource,
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

		// === IN BILL (PDF) CHO SALE ORDER ===
		// GET: /Sale/Bill/{id} — Trang chọn mẫu in + xem thử PDF.
		[HttpGet]
		[Route("Bill/{id}")]
		public async Task<IActionResult> Bill(int id)
		{
			var order = await _salesOrderService.GetByIdAsync(id);
			if (order == null) return NotFound();

			var templates = await _billTemplateRepository.GetAllAsync();
			var defaultTemplate = await _billTemplateRepository.GetDefaultAsync();
			var firstTemplateId = templates.FirstOrDefault()?.Id
				?? defaultTemplate?.Id
				?? 0;

			ViewBag.OrderId = id;
			ViewBag.OrderNumber = order.OrderNumber;
			ViewBag.Templates = templates.Select(t => new
			{
				t.Id,
				t.TemplateName,
				t.IsDefault
			}).ToList();
			ViewBag.DefaultTemplateId = firstTemplateId;

			return View();
		}

		// GET: /Sale/PreviewPdf/{id}?templateId= — trả PDF inline để xem thử / in ra thiết bị.
		[HttpGet]
		[Route("PreviewPdf/{id}")]
		public async Task<IActionResult> PreviewPdf(int id, int templateId)
		{
			var data = await _reportService.BuildReportDataAsync(id, templateId);
			if (data == null) return NotFound();

			// Ghi log khi render/in PDF bill (dùng cho preview lẫn Print to Device).
			await LogAsync($"Printed/previewed bill PDF for Sale Order: {data.OrderNumber} (ID: {id}, TemplateID: {templateId})");

			var bytes = _reportService.RenderPdf(data);
			var fileName = $"SO_{data.OrderNumber}.pdf";
			Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");
			return File(bytes, "application/pdf");
		}

		// POST: /Sale/SavePdf/{id}?templateId= — render + lưu PDF vào AtlasStorage + ghi SalesOrderBills.
		[HttpPost]
		[Route("SavePdf/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SavePdf(int id, int templateId)
		{
			var order = await _salesOrderService.GetByIdAsync(id);
			if (order == null) return NotFound();

			try
			{
				var data = await _reportService.BuildReportDataAsync(id, templateId);
				if (data == null) return NotFound();

				var bytes = _reportService.RenderPdf(data);
				var fileName = $"SO_{order.OrderNumber}.pdf";

				using var stream = new MemoryStream(bytes);
				var relativePath = await _storageProvider.SaveFileAsync(stream, "SaleReports", fileName);

				var bill = new SalesOrderBill
				{
					OrderId = order.Id,
					BillUrl = relativePath,
					BillSource = "Generated"
				};
				await _soBillRepository.AddAsync(bill);

				await LogAsync($"Saved bill PDF for Sale Order: {order.OrderNumber} (ID: {id}, TemplateID: {templateId})");

				TempData["Success"] = "Đã lưu file PDF bill vào AtlasStorage.";
			}
			catch
			{
				TempData["Error"] = "Không thể tạo/lưu file PDF. Vui lòng thử lại.";
			}

			return RedirectToAction(nameof(Details), new { id });
		}

		[HttpGet]
		[Route("Create")]
		public async Task<IActionResult> Create()
		{
			var model = new SaleOrderCreateVM();
			await PopulateCreateLookupsAsync(model);

			// Hiển thị sẵn số SO tiếp theo (chỉ để tham khảo, hệ thống tự sinh khi lưu).
			model.OrderNumber = await _documentNumberService.GenerateSalesOrderNumberAsync();

			return View(model);
		}

		[HttpPost]
		[Route("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(SaleOrderCreateVM model)
		{
			await PopulateCreateLookupsAsync(model);

			// Số SO do hệ thống tự sinh tuần tự, bỏ qua giá trị người dùng nhập.
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
				OrderNumber = await _documentNumberService.GenerateSalesOrderNumberAsync(),
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
							VariantPrice = variant.VariantPrice,
							AttributeText = BuildAttributeText(variant)
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

		// Ghi log hành động của nhân viên đang đăng nhập. Không ném lỗi để tránh ảnh hưởng luồng chính.
		private async Task LogAsync(string message)
		{
			try
			{
				var employeeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (int.TryParse(employeeIdValue, out var employeeId))
				{
					await _logService.AddLogAsync(employeeId, message);
				}
			}
			catch
			{
				// Log là best-effort: không làm gián đoạn nghiệp vụ khi ghi log thất bại.
			}
		}

		// Nối các giá trị thuộc tính của biến thể (vd: "Đỏ, L") để phân biệt các biến thể
		// cùng tên sản phẩm trong dropdown chọn theo tên.
		private static string BuildAttributeText(ProductVariant variant)
		{
			if (variant?.AttributeMappings == null || !variant.AttributeMappings.Any())
			{
				return string.Empty;
			}

			var values = variant.AttributeMappings
				.Where(mapping => mapping.AttributeValue != null)
				.Select(mapping => mapping.AttributeValue!.Value)
				.Where(value => !string.IsNullOrWhiteSpace(value))
				.ToList();

			return string.Join(", ", values);
		}
	}
}

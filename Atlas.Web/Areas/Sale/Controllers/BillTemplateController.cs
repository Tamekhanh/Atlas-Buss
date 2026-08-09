using System.Text.Json;
using System.Security.Claims;
using Atlas.Core.DTOs;
using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Services;
using Atlas.Web.Areas.SaleOrder.Models;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Web.Areas.Sale.Controllers
{
	[Area("Sale")]
	[Route("Sale/BillTemplate")]
	public class BillTemplateController : Controller
	{
		private readonly IBillTemplateRepository _templateRepository;
		private readonly ISalesOrderRepository _orderRepository;
		private readonly ISalesOrderReportService _reportService;
		private readonly ILogService _logService;

		public BillTemplateController(
			IBillTemplateRepository templateRepository,
			ISalesOrderRepository orderRepository,
			ISalesOrderReportService reportService,
			ILogService logService)
		{
			_templateRepository = templateRepository;
			_orderRepository = orderRepository;
			_reportService = reportService;
			_logService = logService;
		}

		// Danh sách mẫu in bill
		[HttpGet]
		[Route("")]
		[Route("Index")]
		public async Task<IActionResult> Index()
		{
			var templates = await _templateRepository.GetAllAsync();
			var model = templates.Select(MapToVM).ToList();
			return View(model);
		}

		[HttpGet]
		[Route("Create")]
		public async Task<IActionResult> Create()
		{
			await PopulateOrdersForPreviewAsync();
			return View(new BillTemplateEditVM());
		}

		[HttpPost]
		[Route("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BillTemplateEditVM model)
		{
			if (!ModelState.IsValid)
			{
				await PopulateOrdersForPreviewAsync();
				return View(model);
			}

			var template = MapFromVM(model);
			await _templateRepository.AddAsync(template);
			await LogAsync($"Created bill template: {template.TemplateName} (ID: {template.Id})");
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		[Route("Edit/{id}")]
		public async Task<IActionResult> Edit(int id)
		{
			var template = await _templateRepository.GetByIdAsync(id);
			if (template == null) return NotFound();

			await PopulateOrdersForPreviewAsync();
			return View(MapToEditVM(template));
		}

		[HttpPost]
		[Route("Edit/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, BillTemplateEditVM model)
		{
			if (id != model.Id) return BadRequest();
			if (!ModelState.IsValid)
			{
				await PopulateOrdersForPreviewAsync();
				return View(model);
			}

			var template = MapFromVM(model);
			template.Id = id;
			await _templateRepository.UpdateAsync(template);
			return RedirectToAction(nameof(Index));
		}

		[Route("Delete/{id}")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			await _templateRepository.DeleteAsync(id);
			return RedirectToAction(nameof(Index));
		}

		// Lấy mẫu in dưới dạng JSON để dùng cho API (modal chọn mẫu trên trang Detail).
		[HttpGet]
		[Route("ListJson")]
		public async Task<IActionResult> ListJson()
		{
			var templates = await _templateRepository.GetAllAsync();
			var data = templates.Select(t => new
			{
				t.Id,
				t.TemplateName,
				t.Description,
				t.PageSize,
				t.Orientation,
				t.IsDefault,
				t.HeaderNote,
				t.FooterNote,
				t.OptionsJson
			});
			return Json(data);
		}

		// Danh sách Sales Order (nhẹ) dạng JSON — nạp dropdown chọn order cho Preview.
		[HttpGet]
		[Route("OrderListJson")]
		public async Task<IActionResult> OrderListJson()
		{
			var orders = await _orderRepository.GetOrderListAsync();
			var data = orders.Select(o => new
			{
				o.Id,
				o.OrderNumber,
				OrderDate = o.OrderDate.ToString("yyyy-MM-dd"),
				o.CustomerName
			});
			return Json(data);
		}

		// POST: /Sale/BillTemplate/Preview — render PDF inline với các tùy chọn CHƯA lưu từ form.
		// Trả về application/pdf (Content-Disposition: inline) để hiển thị trong iframe qua fetch+blob.
		[HttpPost]
		[Route("Preview")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Preview([FromBody] BillTemplatePreviewVM model)
		{
			if (model == null || model.OrderId <= 0)
				return BadRequest("OrderId is required.");

			var options = new BillTemplateOptions
			{
				ShowLogo = model.ShowLogo,
				ShowTaxBreakdown = model.ShowTaxBreakdown,
				ShowSignatureLine = model.ShowSignatureLine,
				ShowGrandTotalBox = model.ShowGrandTotalBox,
				ShowCustomerInfo = model.ShowCustomerInfo,
				ShowWarehouseColumn = model.ShowWarehouseColumn,
				BillTitle = model.BillTitle,
				BillSubtitle = model.BillSubtitle,
				ShowSkuColumn = model.ShowSkuColumn,
				ShowDescriptionColumn = model.ShowDescriptionColumn,
				ShowAmountInWords = model.ShowAmountInWords,
				ShowCurrencyCode = model.ShowCurrencyCode,
				ShowExchangeRate = model.ShowExchangeRate,
				ShowPageNumbers = model.ShowPageNumbers,
				AccentColorHex = model.AccentColorHex,
				LogoMaxHeight = model.LogoMaxHeight,
				PageMargin = model.PageMargin,
				GrandTotalBoxStyle = string.IsNullOrWhiteSpace(model.GrandTotalBoxStyle) ? "Box" : model.GrandTotalBoxStyle
			};

			var data = await _reportService.BuildReportDataAsync(
				model.OrderId, options, model.PageSize, model.Orientation, model.HeaderNote, model.FooterNote);
			if (data == null) return NotFound("Sales Order not found.");

			var bytes = _reportService.RenderPdf(data);
			var fileName = $"Preview_SO_{data.OrderNumber}.pdf";
			Response.Headers.Append("Content-Disposition", $"inline; filename=\"{fileName}\"");
			return File(bytes, "application/pdf");
		}

		// Nạp danh sách order (nhẹ) vào ViewBag để form render dropdown chọn order cho preview.
		private async Task PopulateOrdersForPreviewAsync()
		{
			var orders = await _orderRepository.GetOrderListAsync();
			ViewBag.Orders = orders
				.Select(order => new
				{
					Id = order.Id,
					OrderNumber = order.OrderNumber,
					OrderDate = order.OrderDate,
					CustomerName = order.CustomerName
				})
				.ToList();
		}

		// ===== Mapping helpers =====
		private static BillTemplateOptions ParseOptions(string? json)
		{
			if (string.IsNullOrWhiteSpace(json)) return new BillTemplateOptions();
			try
			{
				return JsonSerializer.Deserialize<BillTemplateOptions>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
					?? new BillTemplateOptions();
			}
			catch { return new BillTemplateOptions(); }
		}

		private static string BuildOptionsJson(BillTemplateEditVM vm)
		{
			var opts = new BillTemplateOptions
			{
				ShowLogo = vm.ShowLogo,
				ShowTaxBreakdown = vm.ShowTaxBreakdown,
				ShowSignatureLine = vm.ShowSignatureLine,
				ShowGrandTotalBox = vm.ShowGrandTotalBox,
				ShowCustomerInfo = vm.ShowCustomerInfo,
				ShowWarehouseColumn = vm.ShowWarehouseColumn,

				// Tiêu đề bill tùy chỉnh
				BillTitle = vm.BillTitle,
				BillSubtitle = vm.BillSubtitle,

				// Cột dòng hàng
				ShowSkuColumn = vm.ShowSkuColumn,
				ShowDescriptionColumn = vm.ShowDescriptionColumn,

				// Trường bổ sung
				ShowAmountInWords = vm.ShowAmountInWords,
				ShowCurrencyCode = vm.ShowCurrencyCode,
				ShowExchangeRate = vm.ShowExchangeRate,
				ShowPageNumbers = vm.ShowPageNumbers,

				// Màu nhấn & kiểu dáng
				AccentColorHex = vm.AccentColorHex,
				LogoMaxHeight = vm.LogoMaxHeight,
				PageMargin = vm.PageMargin,
				GrandTotalBoxStyle = vm.GrandTotalBoxStyle
			};
			return JsonSerializer.Serialize(opts);
		}

		private static BillTemplates MapFromVM(BillTemplateEditVM vm)
		{
			return new BillTemplates
			{
				Id = vm.Id,
				TemplateName = vm.TemplateName,
				Description = vm.Description,
				PageSize = vm.PageSize,
				Orientation = vm.Orientation,
				OptionsJson = BuildOptionsJson(vm),
				HeaderNote = vm.HeaderNote,
				FooterNote = vm.FooterNote,
				IsDefault = vm.IsDefault
			};
		}

		private static BillTemplateVM MapToVM(BillTemplates t)
		{
			var opts = ParseOptions(t.OptionsJson);
			return new BillTemplateVM
			{
				Id = t.Id,
				TemplateName = t.TemplateName,
				Description = t.Description,
				PageSize = t.PageSize,
				Orientation = t.Orientation,
				HeaderNote = t.HeaderNote,
				FooterNote = t.FooterNote,
				IsDefault = t.IsDefault,

				ShowLogo = opts.ShowLogo,
				ShowTaxBreakdown = opts.ShowTaxBreakdown,
				ShowSignatureLine = opts.ShowSignatureLine,
				ShowGrandTotalBox = opts.ShowGrandTotalBox,
				ShowCustomerInfo = opts.ShowCustomerInfo,
				ShowWarehouseColumn = opts.ShowWarehouseColumn,

				BillTitle = opts.BillTitle,
				BillSubtitle = opts.BillSubtitle,
				ShowSkuColumn = opts.ShowSkuColumn,
				ShowDescriptionColumn = opts.ShowDescriptionColumn,
				ShowAmountInWords = opts.ShowAmountInWords,
				ShowCurrencyCode = opts.ShowCurrencyCode,
				ShowExchangeRate = opts.ShowExchangeRate,
				ShowPageNumbers = opts.ShowPageNumbers,
				AccentColorHex = opts.AccentColorHex,
				LogoMaxHeight = opts.LogoMaxHeight,
				PageMargin = opts.PageMargin,
				GrandTotalBoxStyle = opts.GrandTotalBoxStyle
			};
		}

		private static BillTemplateEditVM MapToEditVM(BillTemplates t)
		{
			var opts = ParseOptions(t.OptionsJson);
			return new BillTemplateEditVM
			{
				Id = t.Id,
				TemplateName = t.TemplateName,
				Description = t.Description,
				PageSize = t.PageSize,
				Orientation = t.Orientation,
				HeaderNote = t.HeaderNote,
				FooterNote = t.FooterNote,
				IsDefault = t.IsDefault,

				ShowLogo = opts.ShowLogo,
				ShowTaxBreakdown = opts.ShowTaxBreakdown,
				ShowSignatureLine = opts.ShowSignatureLine,
				ShowGrandTotalBox = opts.ShowGrandTotalBox,
				ShowCustomerInfo = opts.ShowCustomerInfo,
				ShowWarehouseColumn = opts.ShowWarehouseColumn,

				BillTitle = opts.BillTitle,
				BillSubtitle = opts.BillSubtitle,
				ShowSkuColumn = opts.ShowSkuColumn,
				ShowDescriptionColumn = opts.ShowDescriptionColumn,
				ShowAmountInWords = opts.ShowAmountInWords,
				ShowCurrencyCode = opts.ShowCurrencyCode,
				ShowExchangeRate = opts.ShowExchangeRate,
				ShowPageNumbers = opts.ShowPageNumbers,
				AccentColorHex = opts.AccentColorHex,
				LogoMaxHeight = opts.LogoMaxHeight,
				PageMargin = opts.PageMargin,
				GrandTotalBoxStyle = opts.GrandTotalBoxStyle
			};
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
				// Log là best-effort.
			}
		}
	}
}

using System.Text.Json;
using System.Security.Claims;
using Atlas.Core.DTOs;
using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Web.Areas.SaleOrder.Models;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Web.Areas.Sale.Controllers
{
	[Area("Sale")]
	[Route("Sale/BillTemplate")]
	public class BillTemplateController : Controller
	{
		private readonly IBillTemplateRepository _templateRepository;
		private readonly ILogService _logService;

		public BillTemplateController(IBillTemplateRepository templateRepository, ILogService logService)
		{
			_templateRepository = templateRepository;
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
		public IActionResult Create()
		{
			return View(new BillTemplateEditVM());
		}

		[HttpPost]
		[Route("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(BillTemplateEditVM model)
		{
			if (!ModelState.IsValid)
			{
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

			return View(MapToEditVM(template));
		}

		[HttpPost]
		[Route("Edit/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, BillTemplateEditVM model)
		{
			if (id != model.Id) return BadRequest();
			if (!ModelState.IsValid) return View(model);

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
				ShowWarehouseColumn = vm.ShowWarehouseColumn
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
				ShowLogo = opts.ShowLogo,
				ShowTaxBreakdown = opts.ShowTaxBreakdown,
				ShowSignatureLine = opts.ShowSignatureLine,
				ShowGrandTotalBox = opts.ShowGrandTotalBox,
				ShowCustomerInfo = opts.ShowCustomerInfo,
				ShowWarehouseColumn = opts.ShowWarehouseColumn,
				HeaderNote = t.HeaderNote,
				FooterNote = t.FooterNote,
				IsDefault = t.IsDefault
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
				ShowLogo = opts.ShowLogo,
				ShowTaxBreakdown = opts.ShowTaxBreakdown,
				ShowSignatureLine = opts.ShowSignatureLine,
				ShowGrandTotalBox = opts.ShowGrandTotalBox,
				ShowCustomerInfo = opts.ShowCustomerInfo,
				ShowWarehouseColumn = opts.ShowWarehouseColumn,
				HeaderNote = t.HeaderNote,
				FooterNote = t.FooterNote,
				IsDefault = t.IsDefault
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

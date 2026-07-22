using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Web.Areas.Setting.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Atlas.Web.Areas.Setting.Controllers
{
	[Area("Setting")]
	[Authorize(Policy = "Administration")]
	public class SettingController : Controller
	{
		private readonly IMyCompanyInfoRepository _companyRepository;
		private readonly ILogService _logService;

		public SettingController(IMyCompanyInfoRepository companyRepository, ILogService logService)
		{
			_companyRepository = companyRepository;
			_logService = logService;
		}

		// GET: /Setting/Settings — hiển thị form thông tin công ty (dùng cho bill PDF).
		[HttpGet]
		public async Task<IActionResult> Settings()
		{
			var info = await _companyRepository.GetAsync();
			ViewBag.HasLogo = info?.LogoId.HasValue == true;

			return View(MapToVM(info));
		}

		// POST: /Setting/Settings — lưu (insert hoặc update) thông tin công ty, có ghi log.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Settings(MyCompanyInfoVM model)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.HasLogo = model.LogoId.HasValue;
				return View(model);
			}

			var info = MapFromVM(model);
			await _companyRepository.UpdateAsync(info);

			await LogAsync($"Updated company info: {info.CompanyName}");

			TempData["SuccessMessage"] = "Company information saved.";
			return RedirectToAction(nameof(Settings));
		}

		// ===== Mapping =====
		private static MyCompanyInfoVM MapToVM(MyCompanyInfo? info)
		{
			return new MyCompanyInfoVM
			{
				Id = info?.Id ?? 0,
				CompanyName = info?.CompanyName ?? string.Empty,
				TaxId = info?.TaxId,
				Address = info?.Address,
				PhoneNumber = info?.PhoneNumber,
				Email = info?.Email,
				LogoId = info?.LogoId
			};
		}

		private static MyCompanyInfo MapFromVM(MyCompanyInfoVM vm)
		{
			return new MyCompanyInfo
			{
				Id = vm.Id,
				CompanyName = vm.CompanyName,
				TaxId = vm.TaxId,
				Address = vm.Address,
				PhoneNumber = vm.PhoneNumber,
				Email = vm.Email,
				LogoId = vm.LogoId
			};
		}

		// Ghi log hành động của admin đang đăng nhập. Best-effort, không ném lỗi.
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

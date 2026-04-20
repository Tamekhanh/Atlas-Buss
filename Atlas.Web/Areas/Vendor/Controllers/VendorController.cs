using Atlas.Core.Interfaces;
using Atlas.Core.Models;
using Atlas.Web.Areas.Vendor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.Web.Areas.Vendor.Controllers
{
    [Area("Vendor")]
    [Authorize]
    public class VendorController : Controller
    {
        private readonly IVendorCompanyService _vendorCompanyService;
        private readonly IVendorPersonService _vendorPersonService;

        public VendorController(IVendorCompanyService vendorCompanyService, IVendorPersonService vendorPersonService)
        {
            _vendorCompanyService = vendorCompanyService;
            _vendorPersonService = vendorPersonService;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _vendorCompanyService.GetAllAsync();
            var persons = await _vendorPersonService.GetAllAsync();

            var items = companies.Select(MapCompany).Concat(persons.Select(MapPerson)).ToList();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new VendorCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendorCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var created = model.RegistrationType == VendorRegistrationType.Company
                ? await CreateCompanyAsync(model)
                : await CreatePersonAsync(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!created)
            {
                ModelState.AddModelError(string.Empty, "Khong the tao Vendor moi.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Tao moi Vendor thanh cong.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CreateCompanyAsync(VendorCreateViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CompanyName) || string.IsNullOrWhiteSpace(model.TaxId))
            {
                ModelState.AddModelError(string.Empty, "Vui long nhap day du thong tin Vendor Company.");
                return false;
            }

            return await _vendorCompanyService.CreateAsync(new CompanyRegistrationRequest
            {
                CompanyName = model.CompanyName.Trim(),
                TaxId = model.TaxId.Trim(),
                Phone = model.Phone.Trim(),
                Email = model.Email.Trim(),
                AddressType = string.IsNullOrWhiteSpace(model.AddressType) ? "HeadOffice" : model.AddressType.Trim(),
                Street = model.Street?.Trim() ?? string.Empty,
                City = model.City?.Trim() ?? string.Empty,
                State = model.State?.Trim() ?? string.Empty,
                Country = model.Country?.Trim() ?? string.Empty
            });
        }

        private async Task<bool> CreatePersonAsync(VendorCreateViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName) || model.DoB is null)
            {
                ModelState.AddModelError(string.Empty, "Vui long nhap day du thong tin Vendor Person.");
                return false;
            }

            return await _vendorPersonService.CreateAsync(new PersonRegistrationRequest
            {
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                DoB = model.DoB.Value,
                TaxId = string.IsNullOrWhiteSpace(model.TaxId) ? null : model.TaxId.Trim(),
                Phone = model.Phone.Trim(),
                Email = model.Email.Trim(),
                AddressType = string.IsNullOrWhiteSpace(model.AddressType) ? "Home" : model.AddressType.Trim(),
                Street = model.Street?.Trim() ?? string.Empty,
                City = model.City?.Trim() ?? string.Empty,
                State = model.State?.Trim() ?? string.Empty,
                Country = model.Country?.Trim() ?? string.Empty
            });
        }

        private static VendorManagementViewModel MapCompany(Atlas.Core.Entities.VendorCompany company)
        {
            return new VendorManagementViewModel
            {
                RegistrationType = VendorRegistrationType.Company,
                Name = company.Company?.CompanyName ?? string.Empty,
                TaxId = company.Company?.TaxId ?? string.Empty,
                Phone = company.Company?.Contact?.Phone ?? string.Empty,
                Email = company.Company?.Contact?.Email ?? string.Empty,
                Address = BuildAddress(company.Company?.Address)
            };
        }

        private static VendorManagementViewModel MapPerson(Atlas.Core.Entities.VendorPerson person)
        {
            return new VendorManagementViewModel
            {
                RegistrationType = VendorRegistrationType.Person,
                Name = person.Person != null ? $"{person.Person.FirstName} {person.Person.LastName}".Trim() : string.Empty,
                TaxId = person.TaxId ?? string.Empty,
                Phone = person.Person?.Contact?.Phone ?? string.Empty,
                Email = person.Person?.Contact?.Email ?? string.Empty,
                Address = BuildAddress(person.Person?.Address)
            };
        }

        private static string BuildAddress(Atlas.Core.Entities.Addresses? address)
        {
            return address == null
                ? "N/A"
                : $"{address.Street}, {address.City}, {address.State}, {address.Country}";
        }
    }
}

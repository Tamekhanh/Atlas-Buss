using Atlas.Core.Interfaces;
using Atlas.Core.Models;
using Atlas.Web.Areas.Customer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Atlas.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerCompanyService _customerCompanyService;
        private readonly ICustomerPersonService _customerPersonService;
        private readonly ILogService _logService;

        public CustomerController(ICustomerCompanyService customerCompanyService, ICustomerPersonService customerPersonService, ILogService logService)
        {
            _customerCompanyService = customerCompanyService;
            _customerPersonService = customerPersonService;
            _logService = logService;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _customerCompanyService.GetAllAsync();
            var persons = await _customerPersonService.GetAllAsync();

            var items = companies.Select(MapCompany).Concat(persons.Select(MapPerson)).ToList();
            return View(items);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CustomerCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var created = model.RegistrationType == CustomerRegistrationType.Company
                ? await CreateCompanyAsync(model)
                : await CreatePersonAsync(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!created)
            {
                ModelState.AddModelError(string.Empty, "Khong the tao Customer moi.");
                return View(model);
            }



            TempData["SuccessMessage"] = "Tao moi Customer thanh cong.";
            var employeeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(employeeIdValue, out var employeeId))
            {
                await _logService.AddLogAsync(employeeId, model.RegistrationType == CustomerRegistrationType.Company ? $"Created new company customer {model.CompanyName}" : $"Created new person customer {model.FirstName} {model.LastName}");
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CreateCompanyAsync(CustomerCreateViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CompanyName) || string.IsNullOrWhiteSpace(model.TaxId))
            {
                ModelState.AddModelError(string.Empty, "Vui long nhap day du thong tin Customer Company.");
                return false;
            }

            return await _customerCompanyService.CreateAsync(new CompanyRegistrationRequest
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

        private async Task<bool> CreatePersonAsync(CustomerCreateViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName) || model.DoB is null)
            {
                ModelState.AddModelError(string.Empty, "Vui long nhap day du thong tin Customer Person.");
                return false;
            }

            return await _customerPersonService.CreateAsync(new PersonRegistrationRequest
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

        private static CustomerManagementViewModel MapCompany(Atlas.Core.Entities.CustomerCompany company)
        {
            return new CustomerManagementViewModel
            {
                RegistrationType = CustomerRegistrationType.Company,
                Name = company.Company?.CompanyName ?? string.Empty,
                TaxId = company.Company?.TaxId ?? string.Empty,
                Phone = company.Company?.Contact?.Phone ?? string.Empty,
                Email = company.Company?.Contact?.Email ?? string.Empty,
                Address = BuildAddress(company.Company?.Address)
            };
        }

        private static CustomerManagementViewModel MapPerson(Atlas.Core.Entities.CustomerPerson person)
        {
            return new CustomerManagementViewModel
            {
                RegistrationType = CustomerRegistrationType.Person,
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

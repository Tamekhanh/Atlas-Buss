using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Web.Areas.Party.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Atlas.Web.Areas.Party.Controllers
{
    [Area("Party")]
    public class PartyController : Controller
    {
        private readonly IPartyRepository _partyRepository;
        private readonly ILogService _logService;

        public PartyController(IPartyRepository partyRepository, ILogService logService)
        {
            _partyRepository = partyRepository;
            _logService = logService;
        }

        // GET: /Party/Party/Index
        public async Task<IActionResult> Index()
        {
            var parties = await _partyRepository.GetAllAsync();

            var viewModel = parties.Select(p => new PartyListViewModel
            {
                Id = p.Id,
                PartyType = p.PartyType,
                DisplayName = p.DisplayName,
                Phone = p.Contact?.Phone,
                Email = p.Contact?.Email,
                IsCustomer = p.IsCustomer,
                IsVendor = p.IsVendor,
                CreatedAt = p.CreatedAt
            }).ToList();

            return View(viewModel);
        }

        // GET: /Party/Party/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var party = await _partyRepository.GetByIdAsync(id);
            if (party == null) return NotFound();

            return View(party); // Ở đây trả thẳng Entity cũng được vì Detail thường cần show hết
        }

        // GET: /Party/Party/Create
        public IActionResult Create()
        {
            return View(new PartyCreateViewModel());
        }

        // POST: /Party/Party/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PartyCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Map từ ViewModel sang Entity
            var newParty = new Atlas.Core.Entities.Party
            {
                PartyType = model.PartyType,
                DisplayName = model.DisplayName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DoB = model.DoB,
                TaxId = string.IsNullOrWhiteSpace(model.TaxId) ? null : model.TaxId,
                IsCustomer = model.IsCustomer,
                IsVendor = model.IsVendor,

                // EF Core sẽ tự động tạo Address và Contact mới khi lưu Party
                Address = new Addresses
                {
                    AddressType = model.AddressType,
                    Street = model.Street,
                    City = model.City,
                    State = model.State,
                    Country = model.Country
                },
                Contact = new Contacts
                {
                    Phone = model.Phone,
                    Email = model.Email
                }
            };

            //LOG
            var employeeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(employeeIdValue, out var employeeId))
            {
                await _logService.AddLogAsync(employeeId, $"Created new party: {newParty.DisplayName} (ID: {newParty.Id})");
            }

            await _partyRepository.AddAsync(newParty);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Party/Party/Delete/5 
        // (Hàm này bây giờ không cần thiết nếu bạn dùng JavaScript confirm, 
        // nhưng bạn có thể giữ lại nếu muốn có một trang xác nhận riêng)
        // public async Task<IActionResult> Delete(int id)
        // {
        //     var party = await _partyRepository.GetByIdAsync(id);
        //     if (party == null) return NotFound();
        //     return View(party);
        // }

        // POST: /Party/Party/Delete/5
        // Đây là hàm thực sự thực hiện việc xóa
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo mật chống tấn công CSRF
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var party = await _partyRepository.GetByIdAsync(id);
                if (party == null) return NotFound();

                await _partyRepository.DeleteAsync(id);

                //LOG
                var employeeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(employeeIdValue, out var employeeId))
                {
                    await _logService.AddLogAsync(employeeId, $"Deleted party: {party.DisplayName} (ID: {party.Id})");
                }
                TempData["SuccessMessage"] = "Party deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu Party đang được sử dụng ở bảng khác (Foreign Key)
                TempData["ErrorMessage"] = "Cannot delete this party because it is linked to other data.";
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Web.Areas.Party.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Web.Areas.Party.Controllers
{
    [Area("Party")]
    public class PartyController : Controller
    {
        private readonly IPartyRepository _partyRepository;

        public PartyController(IPartyRepository partyRepository)
        {
            _partyRepository = partyRepository;
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

            await _partyRepository.AddAsync(newParty);
            return RedirectToAction(nameof(Index));
        }
    }
}
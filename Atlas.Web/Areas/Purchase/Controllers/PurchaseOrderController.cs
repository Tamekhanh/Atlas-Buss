// Atlas.Web.Controllers.PurchaseOrderController.cs
using Microsoft.AspNetCore.Mvc;
using Atlas.Core.Interfaces;
using Atlas.Core.Entities;
using Atlas.Web.Areas.PurchaseOrder.Models;

namespace Atlas.Web.Controllers
{
    [Area("Purchase")] 
    [Route("Purchase")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderRepository _poRepo;
        // Giả sử bạn có các repo cho Vendor, Variant...
        // private readonly IVendorRepository _vendorRepo; 
        // private readonly IProductVariantRepository _variantRepo;

        public PurchaseOrderController(IPurchaseOrderRepository poRepo)
        {
            _poRepo = poRepo;
        }
        
        [Route("Index")] 
        public async Task<IActionResult> Index()
        {
            var orders = await _poRepo.GetAllAsync();
            // Map Entity -> ViewModel
            var model = orders.Select(x => new PurchaseOrderVM {
                Id = x.Id,
                PONumber = x.PONumber,
                OrderDate = x.OrderDate,
                VendorId = x.VendorId,
            });
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Load danh sách Vendor, Status, Currency vào ViewBag để đổ vào Dropdown
            // ViewBag.Vendors = _vendorRepo.GetAll(); 
            return View(new PurchaseOrderCreateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrderCreateVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var po = new PurchaseOrder
            {
                PONumber = model.PONumber,
                VendorId = model.VendorId,
                OrderDate = model.OrderDate,
                OrderStatusId = model.OrderStatusId,
                CurrencyId = model.CurrencyId,
                ExchangeRate = model.ExchangeRate,
                EmployeeId = 1 // Lấy từ User Session
            };

            foreach (var item in model.OrderDetails)
            {
                po.PurchaseOrderDetails.Add(new PurchaseOrderDetail
                {
                    VariantId = item.VariantId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    TaxAmount = item.TaxAmount,
                    WarehouseId = item.WarehouseId
                });
            }

            var result = await _poRepo.AddAsync(po);
            if (result) return RedirectToAction(nameof(Index));

            return View(model);
        }
    }
}
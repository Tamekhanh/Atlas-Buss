using Microsoft.AspNetCore.Mvc;
using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Services;
using Atlas.Web.Areas.PurchaseOrder.Models;
using System.Linq;

namespace Atlas.Web.Controllers
{
    [Area("Purchase")]
    [Route("Purchase")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _poService;
        private readonly IProductRepository _productRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        // private readonly IVendorRepository _vendorRepo;
        // private readonly IProductVariantRepository _variantRepo;

        public PurchaseOrderController(IPurchaseOrderService poService, IProductRepository productRepository, IWarehouseRepository warehouseRepository)
        {
            _poService = poService;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
        }
        [Route("Index")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _poService.GetAllAsync();
            var model = orders.Select(x => new PurchaseOrderVM
            {
                Id = x.Id,
                PONumber = x.PONumber,
                OrderDate = x.OrderDate,
                VendorId = x.VendorId,
            });
            return View(model);
        }

        [Route("Details/{id}")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _poService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Tải trước danh sách kho để hiển thị tên thay vì chỉ WarehouseId
            var warehouses = (await _warehouseRepository.GetAllAsync())
                .Where(w => !w.IsDeleted)
                .ToDictionary(w => w.Id, w => w.WarehouseName);

            var lines = order.PurchaseOrderDetails
                .OrderBy(d => d.Id)
                .Select(d =>
                {
                    var lineTotal = (d.Quantity * d.UnitPrice) - d.Discount + d.TaxAmount;
                    return new PurchaseOrderDetailLineVM
                    {
                        VariantId = d.VariantId,
                        ProductName = d.Variant?.Product?.ProductName ?? string.Empty,
                        VariantSKU = d.Variant?.SKU ?? string.Empty,
                        WarehouseId = d.WarehouseId,
                        WarehouseName = warehouses.TryGetValue(d.WarehouseId, out var whName) ? whName : string.Empty,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        Discount = d.Discount,
                        TaxAmount = d.TaxAmount,
                        LineTotal = lineTotal
                    };
                })
                .ToList();

            var model = new PurchaseOrderDetailPageVM
            {
                Id = order.Id,
                PONumber = order.PONumber,
                OrderDate = order.OrderDate,
                VendorId = order.VendorId,
                VendorName = order.Vendor?.DisplayName ?? string.Empty,
                EmployeeId = order.EmployeeId,
                EmployeeName = order.Employee?.FullName ?? string.Empty,
                OrderStatusId = order.OrderStatusId,
                StatusName = order.OrderStatus?.StatusName ?? string.Empty,
                CurrencyCode = order.Currency?.CurrencyCode ?? string.Empty,
                ExchangeRate = order.ExchangeRate,
                SubTotal = lines.Sum(l => l.Quantity * l.UnitPrice),
                TotalDiscount = lines.Sum(l => l.Discount),
                TotalTax = lines.Sum(l => l.TaxAmount),
                GrandTotal = lines.Sum(l => l.LineTotal),
                Lines = lines
            };

            return View(model);
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new PurchaseOrderCreateVM();
            PopulateCreateLookupsAsync(model).GetAwaiter().GetResult();
            return View(model);
        }

        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrderCreateVM model)
        {
            await PopulateCreateLookupsAsync(model);

            if (!ModelState.IsValid) return View(model);

            var po = new PurchaseOrder
            {
                PONumber = model.PONumber,
                VendorId = model.VendorId,
                OrderDate = model.OrderDate,
                OrderStatusId = model.OrderStatusId,
                CurrencyId = model.CurrencyId,
                ExchangeRate = model.ExchangeRate,
                EmployeeId = 1, // Lấy từ User Session
                PurchaseOrderDetails = new List<PurchaseOrderDetail>() // Khởi tạo list tránh NullReferenceException
            };

            // Kiểm tra null đề phòng form gửi lên không có OrderDetails nào
            if (model.OrderDetails != null && model.OrderDetails.Any())
            {
                foreach (var item in model.OrderDetails)
                {
                    po.PurchaseOrderDetails.Add(new PurchaseOrderDetail
                    {
                        VariantId = item.VariantId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Discount = item.Discount,
                        TaxAmount = item.TaxAmount,
                        WarehouseId = item.WarehouseId,
                    });
                }
            }

            var result = await _poService.CreateAsync(po);
            if (result) return RedirectToAction(nameof(Index));

            return View(model);
        }

        private async Task PopulateCreateLookupsAsync(PurchaseOrderCreateVM model)
        {
            var products = await _productRepository.GetAllWithVariantsAsync();
            model.Products = products
                .Select(product => new PurchaseOrderProductLookupVM
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Variants = product.Variants
                        .Select(variant => new PurchaseOrderVariantLookupVM
                        {
                            Id = variant.Id,
                            ProductId = variant.ProductId,
                            SKU = variant.SKU,
                            VariantPrice = variant.VariantPrice
                        })
                        .ToList()
                })
                .ToList();
        }
    }
}
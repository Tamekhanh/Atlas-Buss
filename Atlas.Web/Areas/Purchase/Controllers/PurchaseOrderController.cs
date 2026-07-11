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
        private readonly IPartyRepository _partyRepository;
        private readonly IStorageProvider _storageProvider;
        private readonly IPurchaseOrderBillRepository _poBillRepository;
        // private readonly IVendorRepository _vendorRepo;
        // private readonly IProductVariantRepository _variantRepo;

        public PurchaseOrderController(IPurchaseOrderService poService, IProductRepository productRepository, IWarehouseRepository warehouseRepository, IPartyRepository partyRepository, IStorageProvider storageProvider, IPurchaseOrderBillRepository poBillRepository)
        {
            _poService = poService;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _partyRepository = partyRepository;
            _storageProvider = storageProvider;
            _poBillRepository = poBillRepository;
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
                Lines = lines,
                Bills = (await _poBillRepository.GetByOrderIdAsync(order.Id))
                    .Select(b => new PurchaseOrderBillVM
                    {
                        Id = b.Id,
                        BillUrl = b.BillUrl,
                        CreatedAt = b.CreatedAt
                    })
                    .ToList()
            };

            return View(model);
        }

        // Tải lên 1 file bill (PDF, ảnh scan, ...) đính kèm cho Purchase Order
        [Route("UploadBill/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadBill(int id, Microsoft.AspNetCore.Http.IFormFile? billFile)
        {
            var order = await _poService.GetByIdAsync(id);
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
                var relativePath = await _storageProvider.SaveFileAsync(stream, "PurchaseBills", billFile.FileName);

                var bill = new PurchaseOrderBill
                {
                    OrderId = order.Id,
                    BillUrl = relativePath
                };
                await _poBillRepository.AddAsync(bill);
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
            var order = await _poService.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var bill = await _poBillRepository.GetByIdAsync(billId);
            if (bill != null && bill.OrderId == order.Id)
            {
                await _poBillRepository.DeleteAsync(billId);
            }

            return RedirectToAction(nameof(Details), new { id });
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
            var vendors = await _partyRepository.GetVendorsAsync();
            model.Vendors = vendors
                .Select(vendor => new PurchaseOrderVendorLookupVM
                {
                    Id = vendor.Id,
                    DisplayName = vendor.DisplayName
                })
                .ToList();

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

            var warehouses = await _warehouseRepository.GetAllAsync();
            model.Warehouses = warehouses
                .Where(warehouse => !warehouse.IsDeleted)
                .Select(warehouse => new PurchaseOrderWarehouseLookupVM
                {
                    Id = warehouse.Id,
                    WarehouseName = warehouse.WarehouseName
                })
                .ToList();
        }
    }
}
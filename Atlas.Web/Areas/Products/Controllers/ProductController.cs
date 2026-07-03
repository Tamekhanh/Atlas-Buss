using Atlas.Core.Interfaces;
using Atlas.Core.Entities;
using Atlas.Web.Areas.Products.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using ProductEntity = Atlas.Core.Entities.Products;

namespace Atlas.Web.Areas.Products.Controllers
{
    [Area("Products")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStorageProvider _storageProvider;
        private readonly ILogService _logService;

        const int pageSize = 20;

        public ProductController(IProductService productService, ICategoryRepository categoryRepository, IStorageProvider storageProvider, ILogService logService)
        {
            _productService = productService;
            _categoryRepository = categoryRepository;
            _logService = logService;
            _storageProvider = storageProvider;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var currentPage = page.GetValueOrDefault(1);
            var products = (await _productService.GetAllProductsAsync(currentPage, pageSize)).ToList();
            ViewData["CurrentPage"] = currentPage;
            ViewData["HasNext"] = products.Count == pageSize;
            return View("~/Areas/Products/Views/Products/Index.cshtml", products);
        }

        public async Task<IActionResult> Search(string searchTerm, string? category, bool? isActive, bool? onSale, int? page)
        {
            var currentPage = page.GetValueOrDefault(1);
            var all = (await _productService.GetProductFilterAsync(searchTerm, category, isActive, onSale))?.ToList() ?? new List<ProductEntity>();
            var paged = all.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            ViewData["CurrentPage"] = currentPage;
            ViewData["HasNext"] = paged.Count == pageSize && all.Count > (currentPage * pageSize);
            return View("~/Areas/Products/Views/Products/Index.cshtml", paged);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            var model = ToDetailModel(product);
            await PopulateCategoriesAsync(model);
            return View("~/Areas/Products/Views/Products/Detail.cshtml", model);
        }

        [HttpPost]
        [Authorize(Policy = "ProductCreate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detail(int id, ProductModelView model)
        {

            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product is null) return NotFound();

                model.Id = id;

                // 1. XỬ LÝ LƯU FILE ẢNH
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    using (var stream = model.ImageFile.OpenReadStream())
                    {
                        var relativeImagePath = await _storageProvider.SaveFileAsync(stream, "Products", model.ImageFile.FileName);
                        product.ImageUrl = relativeImagePath;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(model.ImageUrl))
                {
                    product.ImageUrl = model.ImageUrl.Trim();
                }
                else
                {
                    product.ImageUrl = null;
                }


                product.BaseSalePrice = model.BaseSalePrice;
                product.BaseCostPrice = model.BaseCostPrice;
                product.Barcode = string.IsNullOrWhiteSpace(model.Barcode) ? null : model.Barcode.Trim();
                product.IsActive = model.IsActive;
                product.Onsale = model.Onsale;

                product.ProductDetail ??= new ProductDetails();
                product.ProductDetail.ProductDescription = string.IsNullOrWhiteSpace(model.ProductDescription) ? null : model.ProductDescription.Trim();
                product.ProductDetail.Weight = model.Weight;
                product.ProductDetail.WarrantyPeriod = model.WarrantyPeriod;
                product.ProductDetail.Dimensions = string.IsNullOrWhiteSpace(model.Dimensions) ? null : model.Dimensions.Trim();
                product.ProductDetail.Manufacturer = string.IsNullOrWhiteSpace(model.Manufacturer) ? null : model.Manufacturer.Trim();

                var updated = await _productService.UpdateProductAsync(product, model.CategoryIds);
                if (!updated)
                {
                    ModelState.AddModelError(string.Empty, "Could not update product.");
                    await PopulateCategoriesAsync(model);
                    return View("~/Areas/Products/Views/Products/Detail.cshtml", model);
                }

                var employeeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(employeeIdValue, out var employeeId))
                {
                    await _logService.AddLogAsync(employeeId, $"Updated product: {product.ProductName} (ID: {product.Id})");
                }
                
                return RedirectToRoute("products", new { action = "Detail", id = product.Id });
            }
            catch (Exception ex)
            {
                // Ghi log ra file hoặc console để xem
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        [HttpGet]
        [Authorize(Policy = "ProductCreate")]
        public async Task<IActionResult> Create()
        {
            var model = new ProductModelView
            {
                EmployeeId = GetCurrentEmployeeId(),
                AvailableCategories = Array.Empty<SelectListItem>()
            };

            await PopulateCategoriesAsync(model);

            // Lấy danh sách các loại thuộc tính để Frontend render dynamic variant
            ViewBag.AttributeTypes = await _productService.GetAvailableAttributeTypesAsync();

            // SỬA: Phải truyền 'model' vào View
            return View("~/Areas/Products/Views/Products/Create.cshtml", model);
        }

        [HttpPost]
        [Authorize(Policy = "ProductCreate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModelView model)
        {
            model.EmployeeId = GetCurrentEmployeeId();
            await PopulateCategoriesAsync(model);

            if (model.EmployeeId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Khong xac dinh duoc nhan vien dang dang nhap.");
            }

            if (!ModelState.IsValid)
            {
                // QUAN TRỌNG: Gán lại ViewBag và TRUYỀN model quay lại View để giữ dữ liệu đã nhập
                ViewBag.AttributeTypes = await _productService.GetAvailableAttributeTypesAsync();
                return View("~/Areas/Products/Views/Products/Create.cshtml", model);
            }

            // Mapping sang Entity Sản phẩm cha
            var product = new ProductEntity
            {
                ProductName = model.ProductName.Trim(),
                ProductCode = model.ProductCode.Trim(),
                UnitId = model.UnitId,
                ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim(),
                BaseSalePrice = model.BaseSalePrice,
                BaseCostPrice = model.BaseCostPrice,
                Barcode = string.IsNullOrWhiteSpace(model.Barcode) ? null : model.Barcode.Trim(),
                IsActive = model.IsActive,
                Onsale = model.Onsale,
                EmployeeId = model.EmployeeId,
                ProductDetail = new ProductDetails
                {
                    ProductDescription = string.IsNullOrWhiteSpace(model.ProductDescription) ? null : model.ProductDescription.Trim(),
                    Weight = model.Weight,
                    WarrantyPeriod = model.WarrantyPeriod,
                    Dimensions = string.IsNullOrWhiteSpace(model.Dimensions) ? null : model.Dimensions.Trim(),
                    Manufacturer = string.IsNullOrWhiteSpace(model.Manufacturer) ? null : model.Manufacturer.Trim()
                }
            };

            // Mapping sang Entity Biến thể (Variants) từ ViewModel
            if (model.Variants != null && model.Variants.Any())
            {
                product.Variants = model.Variants.Select(v => new ProductVariant
                {
                    SKU = v.SKU.Trim(),
                    VariantPrice = v.Price,
                    VariantCost = v.Cost,
                    AttributeMappings = v.AttributeValueIds.Select(avId => new VariantAttributeMapping
                    {
                        AttributeValueId = avId
                    }).ToList()
                }).ToList();
            }

            // Gọi Service tạo sản phẩm
            var created = await _productService.CreateProductAsync(product, model.CategoryIds, product.Variants);
            if (!created)
            {
                ModelState.AddModelError(string.Empty, "Could not create product.");
                ViewBag.AttributeTypes = await _productService.GetAvailableAttributeTypesAsync();
                return View("~/Areas/Products/Views/Products/Create.cshtml", model);
            }

            // LOGGING: Đơn giản hóa vì đã biết 'created' là true
            var employeeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(employeeIdValue, out var employeeId))
            {
                await _logService.AddLogAsync(employeeId, $"Created new product: {product.ProductName} (ID: {product.Id})");
            }

            return RedirectToAction(nameof(Index));
        }

        private int GetCurrentEmployeeId()
        {
            var employeeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(employeeIdValue, out var employeeId) ? employeeId : 0;
        }

        private static ProductModelView ToDetailModel(ProductEntity product)
        {
            return new ProductModelView
            {
                Id = product.Id,
                ProductName = product.ProductName,
                ProductCode = product.ProductCode,

                // SỬA TẠI ĐÂY: Sử dụng toán tử ?? để gán giá trị 0 nếu UnitId bị null
                UnitId = product.UnitId ?? 0,

                ImageUrl = product.ImageUrl,
                BaseSalePrice = product.BaseSalePrice,
                BaseCostPrice = product.BaseCostPrice,
                Barcode = product.Barcode,
                IsActive = product.IsActive,
                Onsale = product.Onsale,
                EmployeeId = product.EmployeeId,
                ProductDescription = product.ProductDetail?.ProductDescription,
                Weight = product.ProductDetail?.Weight,
                WarrantyPeriod = product.ProductDetail?.WarrantyPeriod,
                Dimensions = product.ProductDetail?.Dimensions,
                Manufacturer = product.ProductDetail?.Manufacturer,
                CategoryIds = product.CategoryProducts
                    .Where(categoryProduct => categoryProduct.CategoryId > 0)
                    .Select(categoryProduct => categoryProduct.CategoryId)
                    .Distinct()
                    .ToList(),

                Variants = product.Variants?.Select(v => new VariantModel
                {
                    Id = v.Id,
                    SKU = v.SKU,
                    Price = v.VariantPrice ?? product.BaseSalePrice,
                    Cost = v.VariantCost ?? product.BaseCostPrice,

                    // Giữ lại IDs để phục vụ việc Update/Edit sau này
                    AttributeValueIds = v.AttributeMappings?.Select(m => m.AttributeValueId).ToList() ?? new List<int>(),

                    // MỚI: Map từ ID sang Tên hiển thị
                    // Giả sử AttributeValue có navigation property dẫn về AttributeType
                    AttributeDescriptions = v.AttributeMappings?
                    .Where(m => m.AttributeValue != null)
                    .Select(m => $"{m.AttributeValue?.AttributeType?.AttributeName ?? "Attr"}: {m.AttributeValue?.Value}")
                    .ToList() ?? new List<string>()
                }).ToList() ?? new List<VariantModel>()
            };
        }

        private async Task PopulateCategoriesAsync(ProductModelView model)
        {
            var categories = await _categoryRepository.GetAllAsync();
            model.AvailableCategories = categories
                .OrderBy(category => category.CategoryName)
                .Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName
                })
                .ToList();
        }
    }
}
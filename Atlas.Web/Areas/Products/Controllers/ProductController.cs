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
        private readonly ILogService _logService;

        public ProductController(IProductService productService, ICategoryRepository categoryRepository, ILogService logService)
        {
            _productService = productService;
            _categoryRepository = categoryRepository;
            _logService = logService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View("~/Areas/Products/Views/Products/Index.cshtml", products);
        }

        public async Task<IActionResult> Search(string searchTerm, string? category, bool? isActive, bool? onSale)
        {
            var products = await _productService.GetProductFilterAsync(searchTerm, category, isActive, onSale);
            return View("~/Areas/Products/Views/Products/Index.cshtml", products);
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
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            model.Id = id;

            product.ProductName = model.ProductName.Trim();
            product.ProductCode = model.ProductCode.Trim();
            product.UnitId = model.UnitId > 0 ? model.UnitId : product.UnitId;
            product.ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim();
            product.SalePrice = model.SalePrice;
            product.CostPrice = model.CostPrice;
            product.Barcode = string.IsNullOrWhiteSpace(model.Barcode) ? null : model.Barcode.Trim();
            product.IsActive = model.IsActive;
            product.Onsale = model.Onsale;

            product.ProductDetail ??= new ProductDetails();
            product.ProductDetail.ProductDescription = string.IsNullOrWhiteSpace(model.ProductDescription) ? null : model.ProductDescription.Trim();
            product.ProductDetail.Weight = model.Weight;
            product.ProductDetail.WarrantyPeriod = model.WarrantyPeriod;
            product.ProductDetail.Dimensions = string.IsNullOrWhiteSpace(model.Dimensions) ? null : model.Dimensions.Trim();
            product.ProductDetail.Manufacturer = string.IsNullOrWhiteSpace(model.Manufacturer) ? null : model.Manufacturer.Trim();

            var updated = await _productService.UpdateProductAsync(product, model.CategoryIds, model.NewCategoryName);
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
                return View("~/Areas/Products/Views/Products/Create.cshtml", model);
            }

            var product = new ProductEntity
            {
                ProductName = model.ProductName.Trim(),
                ProductCode = model.ProductCode.Trim(),
                UnitId = model.UnitId,
                ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim(),
                SalePrice = model.SalePrice,
                CostPrice = model.CostPrice,
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

            var created = await _productService.CreateProductAsync(product, model.CategoryIds, model.NewCategoryName);
            if (!created)
            {
                ModelState.AddModelError(string.Empty, "Could not create product.");
                return View("~/Areas/Products/Views/Products/Create.cshtml", model);
            }

            var employeeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (created && int.TryParse(employeeIdValue, out var employeeId))
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
                UnitId = product.UnitId,
                ImageUrl = product.ImageUrl,
                SalePrice = product.SalePrice,
                CostPrice = product.CostPrice,
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
                    .ToList()
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
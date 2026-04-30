using Atlas.Core.Interfaces;
using Atlas.Core.Entities;
using Atlas.Web.Areas.Products.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProductEntity = Atlas.Core.Entities.Products;

namespace Atlas.Web.Areas.Products.Controllers
{
    [Area("Products")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View("~/Areas/Products/Views/Products/Index.cshtml", products);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product is null)
            {
                return NotFound();
            }

            return View("~/Areas/Products/Views/Products/Detail.cshtml", product);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Areas/Products/Views/Products/Create.cshtml", new ProductModelView
            {
                EmployeeId = GetCurrentEmployeeId()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModelView model)
        {
            model.EmployeeId = GetCurrentEmployeeId();

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

            var created = await _productService.CreateProductAsync(product);
            if (!created)
            {
                ModelState.AddModelError(string.Empty, "Could not create product.");
                return View("~/Areas/Products/Views/Products/Create.cshtml", model);
            }

            return RedirectToAction(nameof(Index));
        }

        private int GetCurrentEmployeeId()
        {
            var employeeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(employeeIdValue, out var employeeId) ? employeeId : 0;
        }
    }
}
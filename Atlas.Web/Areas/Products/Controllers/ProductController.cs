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
        private readonly IImageRepository _imageRepository;

        const int pageSize = 20;

        public ProductController(IProductService productService, ICategoryRepository categoryRepository, IStorageProvider storageProvider, ILogService logService, IImageRepository imageRepository)
        {
            _productService = productService;
            _categoryRepository = categoryRepository;
            _logService = logService;
            _storageProvider = storageProvider;
            _imageRepository = imageRepository;
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
            ViewBag.AttributeTypes = await _productService.GetAvailableAttributeTypesAsync();
            await PopulateCategoriesAsync(model);
            return View("~/Areas/Products/Views/Products/Detail.cshtml", model);
        }

        [HttpPost]
        [Authorize(Policy = "ProductManage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detail(int id, ProductModelView model)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product is null) return NotFound();

                // 1. Lấy danh sách ImageIds từ form (Đây là những ảnh KHÔNG bị xóa)
                List<int> finalImageIds = model.ImageIds ?? new List<int>();

                // 2. Xử lý upload các file mới (nếu có)
                if (model.ImageFile != null && model.ImageFile.Any())
                {
                    foreach (var file in model.ImageFile)
                    {
                        if (file.Length > 0)
                        {
                            using var stream = file.OpenReadStream();
                            var path = await _storageProvider.SaveFileAsync(stream, "Products", file.FileName);
                            var image = new Images { ImageUrl = path };
                            await _imageRepository.AddAsync(image);
                            finalImageIds.Add(image.Id);
                        }
                    }
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

                product.Variants = MapVariants(model.Variants, product.Id);

                var updated = await _productService.UpdateProductAsync(product, model.CategoryIds, finalImageIds, product.Variants);
                if (!updated)
                {
                    ModelState.AddModelError(string.Empty, "Could not update product.");
                    ViewBag.AttributeTypes = await _productService.GetAvailableAttributeTypesAsync();
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
        [Authorize(Policy = "ProductManage")]
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
        [Authorize(Policy = "ProductManage")]
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
                ViewBag.AttributeTypes = await _productService.GetAvailableAttributeTypesAsync();
                return View("~/Areas/Products/Views/Products/Create.cshtml", model);
            }

            // --- BƯỚC 1: XỬ LÝ LƯU ẢNH VÀ LẤY IMAGE IDS ---
            List<int> imageIds = new List<int>();
            if (model.ImageFile != null && model.ImageFile.Any())
            {
                foreach (var file in model.ImageFile) // Bây giờ model.ImageFiles là List nên foreach sẽ chạy được
                {
                    if (file.Length > 0)
                    {
                        using (var stream = file.OpenReadStream())
                        {
                            var relativeImagePath = await _storageProvider.SaveFileAsync(stream, "Products", file.FileName);
                            var image = new Images { ImageUrl = relativeImagePath };
                            await _imageRepository.AddAsync(image);
                            imageIds.Add(image.Id);
                        }
                    }
                }
            }

            // Mapping sang Entity Sản phẩm cha
            var product = new ProductEntity
            {
                ProductName = model.ProductName.Trim(),
                ProductCode = model.ProductCode.Trim(),
                UnitId = model.UnitId,
                // Bỏ dòng product.ImageUrl = ... (Vì giờ dùng bảng Images)
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

            // Mapping sang Entity Biến thể
            if (model.Variants != null && model.Variants.Any())
            {
                product.Variants = MapVariants(model.Variants, 0);
            }

            // --- BƯỚC 2: GỌI SERVICE VỚI ĐÚNG THỨ TỰ THAM SỐ ---
            // Thứ tự: product, categoryIds, imageIds, variants
            var created = await _productService.CreateProductAsync(product, model.CategoryIds, imageIds, product.Variants);

            if (!created)
            {
                ModelState.AddModelError(string.Empty, "Could not create product.");
                ViewBag.AttributeTypes = await _productService.GetAvailableAttributeTypesAsync();
                return View("~/Areas/Products/Views/Products/Create.cshtml", model);
            }

            // LOGGING...
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
                UnitId = product.UnitId ?? 0,

                ImageUrl = product.ProductImages?.FirstOrDefault()?.Image?.ImageUrl,

                // 2. Lấy tất cả ID ảnh để lưu giữ khi Submit Form

                CurrentImages = product.ProductImages?.Select(pi => new ImageItemModel
                {
                    Id = pi.ImageId,
                    Url = pi.Image?.ImageUrl ?? ""
                }).ToList() ?? new List<ImageItemModel>(),

                ImageIds = product.ProductImages?
                        .Select(pi => pi.ImageId)
                        .ToList() ?? new List<int>(),

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
                    AttributeValueIds = v.AttributeMappings?.Select(m => m.AttributeValueId).ToList() ?? new List<int>(),
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

        private static List<ProductVariant> MapVariants(IEnumerable<VariantModel>? variants, int productId)
        {
            return (variants ?? Enumerable.Empty<VariantModel>())
                .Select(v => new ProductVariant
                {
                    Id = v.Id,
                    ProductId = productId,
                    SKU = v.SKU?.Trim() ?? string.Empty,
                    VariantPrice = v.Price,
                    VariantCost = v.Cost,
                    AttributeMappings = (v.AttributeValueIds ?? new List<int>())
                        .Where(attributeValueId => attributeValueId > 0)
                        .Distinct()
                        .Select(attributeValueId => new VariantAttributeMapping
                        {
                            AttributeValueId = attributeValueId
                        })
                        .ToList()
                })
                .Where(variant => !string.IsNullOrWhiteSpace(variant.SKU))
                .ToList();
        }

    }
}
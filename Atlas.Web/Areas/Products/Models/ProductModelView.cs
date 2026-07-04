using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.AspNetCore.Http; // Thêm cái này để dùng IFormFile

namespace Atlas.Web.Areas.Products.Models
{
    public class VariantModel
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public List<int> AttributeValueIds { get; set; } = new();
        public List<string> AttributeDescriptions { get; set; } = new();
    }

    public class ImageItemModel
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
    }

    public class ProductModelView
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Unit is required.")]
        public int UnitId { get; set; }

        public string? ImageUrl { get; set; } // Để hiển thị preview
        public List<ImageItemModel> CurrentImages { get; set; } = new();
        public List<IFormFile>? ImageFile { get; set; }
        public List<int> ImageIds { get; set; } = new(); // ĐỂ GIỮ LẠI ẢNH CŨ KHI UPDATE

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Base sale price must be greater than 0.")]
        public decimal BaseSalePrice { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Base cost price must be greater than 0.")]
        public decimal BaseCostPrice { get; set; }

        [StringLength(50)]
        public string? Barcode { get; set; }

        public bool IsActive { get; set; } = true;
        public bool Onsale { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [StringLength(255)]
        public string? ProductDescription { get; set; }

        public decimal? Weight { get; set; }
        public int? WarrantyPeriod { get; set; }
        [StringLength(50)]
        public string? Dimensions { get; set; }
        [StringLength(100)]
        public string? Manufacturer { get; set; }

        public List<int> CategoryIds { get; set; } = new();
        [StringLength(100)]
        public string? NewCategoryName { get; set; }
        public IEnumerable<SelectListItem> AvailableCategories { get; set; } = Enumerable.Empty<SelectListItem>();

        public List<VariantModel> Variants { get; set; } = new();
    }
}
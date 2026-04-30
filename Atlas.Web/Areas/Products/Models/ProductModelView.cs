using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Atlas.Web.Areas.Products.Models
{
    public class ProductModelView
    {
        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; } = string.Empty;

        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Sale price must be greater than 0.")]
        public decimal SalePrice { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost price must be greater than 0.")]
        public decimal CostPrice { get; set; }

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
    }
}
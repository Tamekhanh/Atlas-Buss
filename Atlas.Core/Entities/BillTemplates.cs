using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas.Core.Entities
{
    // Bảng dbo.BillTemplates (mẫu in bill tùy chỉnh cho Sales Order)
    // Đã được tạo sẵn trong SQLDB.sql
    [Table("BillTemplates")]
    public class BillTemplates
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string TemplateName { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        // Kích thước giấy: A4 | A5 | Letter
        [Required]
        [StringLength(20)]
        public string PageSize { get; set; } = "A4";

        // Hướng giấy: Portrait | Landscape
        [Required]
        [StringLength(10)]
        public string Orientation { get; set; } = "Portrait";

        // JSON chứa các tùy chọn in (showLogo, showTaxBreakdown, showSignatureLine, ...)
        public string? OptionsJson { get; set; }

        [StringLength(500)]
        public string? HeaderNote { get; set; }

        [StringLength(500)]
        public string? FooterNote { get; set; }

        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}

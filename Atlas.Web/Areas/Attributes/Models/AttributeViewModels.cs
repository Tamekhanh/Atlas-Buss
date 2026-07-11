using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Web.Areas.Attributes.Models
{
    // Dùng cho trang quản lý Thuộc tính (Attributes).
    public class AttributeTypeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Attribute name is required.")]
        [StringLength(50, ErrorMessage = "Attribute name cannot exceed 50 characters.")]
        public string AttributeName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // Các giá trị thuộc tính (Màu Đỏ, Xanh...) - null khi chỉ cần header.
        public List<AttributeValueViewModel> Values { get; set; } = new();
    }

    public class AttributeValueViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Value is required.")]
        [StringLength(50, ErrorMessage = "Value cannot exceed 50 characters.")]
        public string Value { get; set; } = string.Empty;

        public int AttributeTypeId { get; set; }
    }
}

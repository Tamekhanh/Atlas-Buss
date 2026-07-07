using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas.Core.Entities
{
    // 1. Sản phẩm chính (Parent Product)
    public class Products
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public int? UnitId { get; set; }
        public decimal BaseSalePrice { get; set; }
        public decimal BaseCostPrice { get; set; }
        public string? Barcode { get; set; }
        public bool IsActive { get; set; } = true;
        public bool Onsale { get; set; } = false;
        public int EmployeeId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<ProductImages> ProductImages { get; set; } = new List<ProductImages>();

        // Navigation Properties
        public Employee? Employee { get; set; }
        public ProductDetails? ProductDetail { get; set; }
        public Units? Unit { get; set; }
        public ICollection<ProductTax> ProductTaxes { get; set; } = new List<ProductTax>();
        public ICollection<CategoryProduct> CategoryProducts { get; set; } = new List<CategoryProduct>();
        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();

        // Mối quan hệ 1-n với Biến thể sản phẩm
        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }

    // 2. Biến thể sản phẩm (Product Variant/SKU)
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; } = null!;
        public decimal? VariantPrice { get; set; }
        public decimal? VariantCost { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public Products? Product { get; set; }
        public ICollection<VariantAttributeMapping> AttributeMappings { get; set; } = new List<VariantAttributeMapping>();

        // BỔ SUNG 2 DÒNG NÀY ĐỂ KẾT NỐI VỚI MODULE KHO (Tránh lỗi biên dịch)
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
        public ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();
    }

    // 3. Loại thuộc tính (ví dụ: Màu sắc, Kích thước)
    public class AttributeType
    {
        public int Id { get; set; }
        public string AttributeName { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<AttributeValue> Values { get; set; } = new List<AttributeValue>();
    }

    // 4. Giá trị thuộc tính (Ví dụ: Đỏ, Xanh, L, XL)
    public class AttributeValue
    {
        public int Id { get; set; }
        public int AttributeTypeId { get; set; }

        [Column("AttributeValue")]
        public string Value { get; set; } = null!;

        public AttributeType? AttributeType { get; set; }
        public ICollection<VariantAttributeMapping> VariantMappings { get; set; } = new List<VariantAttributeMapping>();
    }

    // 5. Bảng trung gian nối Biến thể với Giá trị thuộc tính
    public class VariantAttributeMapping
    {
        public int VariantId { get; set; }
        public int AttributeValueId { get; set; }

        public ProductVariant? ProductVariant { get; set; }
        public AttributeValue? AttributeValue { get; set; }
    }

    public class ProductImages
    {
        // Khóa ngoại nối tới bảng Products
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Products? Product { get; set; }

        // Khóa ngoại nối tới bảng Images
        public int ImageId { get; set; }
        [ForeignKey("ImageId")]
        public virtual Images? Image { get; set; }
    }

    // 6. Chi tiết sản phẩm (Giữ nguyên nhưng cập nhật link)
    public class ProductDetails
    {
        public int ProductId { get; set; }
        public string? ProductDescription { get; set; }
        public decimal? Weight { get; set; }
        public int? WarrantyPeriod { get; set; }
        public string? Dimensions { get; set; }
        public string? Manufacturer { get; set; }

        public Products? Product { get; set; }
    }
}
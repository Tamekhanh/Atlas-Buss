using System;
using System.Collections.Generic;

namespace Atlas.Core.Entities
{
    public class Pricelist
    {
        public int Id { get; set; }
        public string PricelistName { get; set; } = string.Empty; // Cần có tên cho bảng giá
        public DateTime EffectiveDate { get; set; } // SQL dùng Date nhưng C# map với DateTime là phổ biến nhất
        public DateTime? ExpiryDate { get; set; }
        
        // CHUẨN HÓA: Thay thế VendorCompanyId/VendorPersonId
        public int? VendorId { get; set; } 
        
        public int CurrencyId { get; set; } = 1;

        // Navigation Properties
        public Party? Vendor { get; set; }
        public Currencies? Currency { get; set; }
        
        public ICollection<PricelistProductVariant> PricelistVariants { get; set; } = new List<PricelistProductVariant>();
    }

    public class PricelistProductVariant
    {
        public int Id { get; set; }
        public int PricelistId { get; set; }
        
        // CHUẨN HÓA: Bảng giá áp dụng cho từng SKU/Variant cụ thể
        public int VariantId { get; set; }
        
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }

        // Navigation Properties
        public Pricelist? Pricelist { get; set; }
        public ProductVariant? Variant { get; set; }
    }
}
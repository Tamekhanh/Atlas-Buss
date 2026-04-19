namespace Atlas.Core.Entities
{
    public class Products
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }
        public string? Barcode { get; set; }
        public bool IsActive { get; set; } = true;
        public bool Onsale { get; set; } = false;
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }
        public ProductDetails? ProductDetail { get; set; }
    }

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

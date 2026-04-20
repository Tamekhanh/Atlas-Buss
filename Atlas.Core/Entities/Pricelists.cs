namespace Atlas.Core.Entities
{
    public class Pricelist
    {
        public int Id { get; set; }
        public DateOnly EffectiveDate { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public int? VendorCompanyId { get; set; }
        public int? VendorPersonId { get; set; }

        public VendorCompany? VendorCompany { get; set; }
        public VendorPerson? VendorPerson { get; set; }
        public ICollection<CategoryPricelist> CategoryPricelists { get; set; } = new List<CategoryPricelist>();
        public ICollection<PricelistProduct> PricelistProducts { get; set; } = new List<PricelistProduct>();
    }

    public class PricelistProduct
    {
        public int Id { get; set; }
        public bool IsStackable { get; set; }
        public int PricelistId { get; set; }
        public int ProductId { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }

        public Pricelist? Pricelist { get; set; }
        public Products? Product { get; set; }
    }
}

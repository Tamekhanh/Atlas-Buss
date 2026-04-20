namespace Atlas.Core.Entities
{
    public class Tax
    {
        public int Id { get; set; }
        public string TaxName { get; set; } = string.Empty;
        public decimal TaxRate { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsStackable { get; set; }

        public ICollection<ProductTax> ProductTaxes { get; set; } = new List<ProductTax>();
    }

    public class ProductTax
    {
        public int ProductId { get; set; }
        public int TaxId { get; set; }

        public Products? Product { get; set; }
        public Tax? Tax { get; set; }
    }
}

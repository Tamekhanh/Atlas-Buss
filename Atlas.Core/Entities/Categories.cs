namespace Atlas.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryDesc { get; set; }

        public ICollection<CategoryPricelist> CategoryPricelists { get; set; } = new List<CategoryPricelist>();
        public ICollection<CategoryProduct> CategoryProducts { get; set; } = new List<CategoryProduct>();
    }

    public class CategoryPricelist
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int PricelistId { get; set; }

        public Category? Category { get; set; }
        public Pricelist? Pricelist { get; set; }
    }

    public class CategoryProduct
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }

        public Category? Category { get; set; }
        public Products? Product { get; set; }
    }
}

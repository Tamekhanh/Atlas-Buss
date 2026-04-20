namespace Atlas.Core.Entities
{
    public class Warehouse
    {
        public int Id { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int AddressId { get; set; }
        public int? ManagerId { get; set; }

        public Addresses? Address { get; set; }
        public Employee? Manager { get; set; }
        public ICollection<InventoryStock> InventoryStocks { get; set; } = new List<InventoryStock>();
    }

    public class InventoryStock
    {
        public int WarehouseId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public Warehouse? Warehouse { get; set; }
        public Products? Product { get; set; }
    }
}

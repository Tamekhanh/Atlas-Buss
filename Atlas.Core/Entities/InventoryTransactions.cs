namespace Atlas.Core.Entities
{
    public class InventoryTransaction
    {
        public long Id { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public string TransactionType { get; set; } = string.Empty; // 'SALE', 'PURCHASE', 'ADJUSTMENT', 'TRANSFER'
        public string? ReferenceId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public string? Note { get; set; }

        public Products? Product { get; set; }
        public Warehouse? Warehouse { get; set; }
        public Employee? Employee { get; set; }
    }
}

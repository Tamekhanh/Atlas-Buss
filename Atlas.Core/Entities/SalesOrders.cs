namespace Atlas.Core.Entities
{
    public class SalesOrder
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int EmployeeId { get; set; }
        public int? CustomerCompanyId { get; set; }
        public int? CustomerPersonId { get; set; }
        public string OrderStatus { get; set; } = "Pending";

        public Employee? Employee { get; set; }
        public CustomerCompany? CustomerCompany { get; set; }
        public CustomerPerson? CustomerPerson { get; set; }
        public ICollection<SalesOrderDetail> SalesOrderDetails { get; set; } = new List<SalesOrderDetail>();
    }

    public class SalesOrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxRate { get; set; }

        public decimal SubTotal => (Quantity * UnitPrice) - Discount;
        public decimal TaxAmount => ((Quantity * UnitPrice) - Discount) * (TaxRate / 100.0m);
        public decimal LineTotal => ((Quantity * UnitPrice) - Discount) * (1 + TaxRate / 100.0m);

        public SalesOrder? SalesOrder { get; set; }
        public Products? Product { get; set; }
        public Warehouse? Warehouse { get; set; }
    }
}

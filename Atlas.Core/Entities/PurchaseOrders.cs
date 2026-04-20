namespace Atlas.Core.Entities
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public int EmployeeId { get; set; }
        public int? VendorCompanyId { get; set; }
        public int? VendorPersonId { get; set; }
        public string OrderStatus { get; set; } = "Draft";

        public Employee? Employee { get; set; }
        public VendorCompany? VendorCompany { get; set; }
        public VendorPerson? VendorPerson { get; set; }
        public ICollection<PurchaseOrderDetail> PurchaseOrderDetails { get; set; } = new List<PurchaseOrderDetail>();
    }

    public class PurchaseOrderDetail
    {
        public int Id { get; set; }
        public int POId { get; set; }
        public int ProductId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TaxRate { get; set; }

        public decimal SubTotal => Quantity * UnitPrice;
        public decimal TaxAmount => (Quantity * UnitPrice) * (TaxRate / 100.0m);
        public decimal LineTotal => (Quantity * UnitPrice) * (1 + TaxRate / 100.0m);

        public PurchaseOrder? PurchaseOrder { get; set; }
        public Products? Product { get; set; }
        public Warehouse? Warehouse { get; set; }
    }
}

namespace Atlas.Core.DTOs
{
    public class PurchaseOrderDTO
    {
        public int Id { get; set; }
        public string PONumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class PurchaseOrderCreateDTO
    {
        public string PONumber { get; set; } = string.Empty;
        public int VendorId { get; set; }
        public int OrderStatusId { get; set; }
        public int CurrencyId { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime OrderDate { get; set; }
        public List<PurchaseOrderDetailDTO> Details { get; set; } = new();
    }

    public class PurchaseOrderDetailDTO
    {
        public int ProductId { get; set; }
        public int VariantId { get; set; }
        public int WarehouseId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TaxAmount { get; set; }
    }
}
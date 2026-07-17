using System;

namespace Atlas.Web.Areas.Warehouse.Models
{
    /// <summary>
    /// Thông tin một dòng tồn kho của kho, hiển thị ở trang Details.
    /// </summary>
    public class WarehouseStockLineViewModel
    {
        public int VariantId { get; set; }
        public string? VariantSKU { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity => Quantity - ReservedQuantity;
        public DateTime LastUpdated { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace Atlas.Core.Entities
{
    public class Currencies
    {
        public int Id { get; set; }
        public string CurrencyCode { get; set; } = null!; // VD: USD, VND
        public string CurrencyName { get; set; } = null!; // VD: US Dollar, Việt Nam Đồng
        public decimal ExchangeRate { get; set; } = 1.0m;
        public bool IsBaseCurrency { get; set; } = false;
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
        public ICollection<Pricelist> Pricelists { get; set; } = new List<Pricelist>();
    }
}
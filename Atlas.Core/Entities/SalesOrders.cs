using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas.Core.Entities
{
    [Table("SalesOrders")]
    public class SalesOrder
    {
        [Key]
        public int Id { get; set; }

        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public int EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee? Employee { get; set; }

        public int CustomerId { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Party? Customer { get; set; }

        public int OrderStatusId { get; set; } = 1;

        [ForeignKey(nameof(OrderStatusId))]
        public virtual SalesOrderStatuses? OrderStatus { get; set; }

        public int CurrencyId { get; set; } = 1;

        [ForeignKey(nameof(CurrencyId))]
        public virtual Currencies? Currency { get; set; }

        public decimal ExchangeRate { get; set; } = 1.0m;
        public decimal TotalDiscount { get; set; } = 0;
        public decimal TotalTax { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; } = new List<SalesOrderDetail>();
    }

    [Table("SalesOrderDetails")]
    public class SalesOrderDetail
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual SalesOrder? SalesOrder { get; set; }

        public int VariantId { get; set; }

        [ForeignKey(nameof(VariantId))]
        public virtual ProductVariant? Variant { get; set; }

        public int WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public virtual Warehouse? Warehouse { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal SubTotal { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal LineTotal { get; set; }

        public bool IsDeleted { get; set; } = false;
    }

    [Table("SalesOrderStatuses")]
    public class SalesOrderStatuses
    {
        [Key]
        public int Id { get; set; }

        public string StatusName { get; set; } = null!;
        public string? Description { get; set; }

        public virtual ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    }
}
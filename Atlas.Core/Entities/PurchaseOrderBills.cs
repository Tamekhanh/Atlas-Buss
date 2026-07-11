using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas.Core.Entities
{
    // Bảng dbo.PurchaseOrderBills (lưu nhiều file bill đính kèm cho 1 PurchaseOrder)
    // Mirror cấu trúc dbo.SalesOrderBills trong SQLDB.sql
    [Table("PurchaseOrderBills")]
    public class PurchaseOrderBill
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual PurchaseOrder? PurchaseOrder { get; set; }

        [Required]
        [StringLength(255)]
        public string BillUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

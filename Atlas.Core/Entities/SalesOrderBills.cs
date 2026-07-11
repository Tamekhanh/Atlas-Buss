using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atlas.Core.Entities
{
    // Bảng dbo.SalesOrderBills (lưu nhiều file bill đính kèm cho 1 SalesOrder)
    // Bảng này đã được tạo sẵn trong SQLDB.sql
    [Table("SalesOrderBills")]
    public class SalesOrderBill
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual SalesOrder? SalesOrder { get; set; }

        [Required]
        [StringLength(255)]
        public string BillUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

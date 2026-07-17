using System;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Web.Areas.Warehouse.Models
{
    public class WarehouseListViewModel
    {
        public int Id { get; set; }
        public string WarehouseName { get; set; } = null!;
        public string? ManagerName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public int InventoryItemCount { get; set; }
    }
}

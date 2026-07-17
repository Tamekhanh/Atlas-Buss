using System.ComponentModel.DataAnnotations;

namespace Atlas.Web.Areas.Warehouse.Models
{
    public class WarehouseCreateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Warehouse name is required")]
        public string WarehouseName { get; set; } = null!;

        // Người quản lý kho (Employee). Có thể để trống.
        public int? ManagerId { get; set; }

        
        // Địa chỉ kho
        public string? AddressType { get; set; } = "Warehouse";
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; } = "Vietnam";
    }
}

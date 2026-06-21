using System;
using System.ComponentModel.DataAnnotations;

namespace Atlas.Web.Areas.Party.Models
{
    public class PartyCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn loại đối tác")]
        public string PartyType { get; set; } = "Person"; // "Person" hoặc "Company"

        [Required(ErrorMessage = "Tên hiển thị là bắt buộc")]
        public string DisplayName { get; set; } = null!;

        // Dành cho Person
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DoB { get; set; }

        public string? TaxId { get; set; }
        public bool IsCustomer { get; set; }
        public bool IsVendor { get; set; }

        // Thông tin liên hệ
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }

        // Địa chỉ
        public string? AddressType { get; set; } = "Office";
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; } = "Vietnam";
    }
}
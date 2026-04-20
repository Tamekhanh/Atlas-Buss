using System.ComponentModel.DataAnnotations;

namespace Atlas.Web.Areas.Customer.Models
{
    public class CustomerCreateViewModel
    {
        [Required]
        public CustomerRegistrationType RegistrationType { get; set; } = CustomerRegistrationType.Company;

        [StringLength(100)]
        public string? CompanyName { get; set; }

        [StringLength(20)]
        public string? TaxId { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DoB { get; set; }

        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string AddressType { get; set; } = "HeadOffice";

        [StringLength(255)]
        public string? Street { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }
    }
}

namespace Atlas.Web.Areas.Vendor.Models
{
    public enum VendorRegistrationType
    {
        Company = 0,
        Person = 1
    }

    public class VendorManagementViewModel
    {
        public VendorRegistrationType RegistrationType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TaxId { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}

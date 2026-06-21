namespace Atlas.Core.Models
{
    public class AddressDto
    {
        public string AddressType { get; set; } = "Office"; // Mặc định có thể là Office, Home, Warehouse...
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class ContactDto
    {
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; } // Email có thể null đối với một số đối tác/cá nhân
    }
}
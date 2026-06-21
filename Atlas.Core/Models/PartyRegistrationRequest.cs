namespace Atlas.Core.Models
{
    public class PartyRegistrationRequest
    {
        // "Person" hoặc "Company"
        public string PartyType { get; set; } = null!; 
        public string DisplayName { get; set; } = null!;
        
        // Thông tin cá nhân (Nullable, chỉ dùng nếu PartyType = "Person")
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DoB { get; set; }
        
        // Thông tin công ty / Thuế
        public string? TaxId { get; set; }
        
        // Vai trò của đối tác
        public bool IsCustomer { get; set; }
        public bool IsVendor { get; set; }

        // Thông tin liên hệ
        public AddressDto Address { get; set; } = null!;
        public ContactDto Contact { get; set; } = null!;
    }
}
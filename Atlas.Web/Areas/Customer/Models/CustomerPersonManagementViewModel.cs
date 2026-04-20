namespace Atlas.Web.Areas.Customer.Models
{
    public class CustomerPersonManagementViewModel
    {
        public int CustomerPersonId { get; set; }
        public int PersonId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DoB { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}

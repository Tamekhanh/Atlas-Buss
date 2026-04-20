using System.ComponentModel.DataAnnotations;

namespace Atlas.Web.Areas.Account.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui long nhap username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui long nhap password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Atlas.Web.Areas.Account.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui long nhap employee number")]
        [Display(Name = "Employee Number")]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui long nhap username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui long nhap password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui long nhap confirm password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Confirm password khong khop")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

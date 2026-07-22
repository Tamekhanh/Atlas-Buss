using System.ComponentModel.DataAnnotations;

namespace Atlas.Web.Areas.Setting.ViewModels
{
	public class MyCompanyInfoVM
	{
		public int Id { get; set; }

		[Required, StringLength(100)]
		public string CompanyName { get; set; } = string.Empty;

		[StringLength(20)]
		public string? TaxId { get; set; }

		[StringLength(255)]
		public string? Address { get; set; }

		[StringLength(20)]
		[Display(Name = "Phone")]
		public string? PhoneNumber { get; set; }

		[StringLength(50)]
		[EmailAddress]
		public string? Email { get; set; }

		// Giữ lại LogoId hiện tại để không bị xoá khi lưu các trường text.
		public int? LogoId { get; set; }
	}
}

using System.ComponentModel.DataAnnotations;

namespace Sauron.ViewModels
{
	public class ForgotViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}
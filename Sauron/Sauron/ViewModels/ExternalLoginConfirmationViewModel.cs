using System.ComponentModel.DataAnnotations;

namespace Sauron.ViewModels
{
	public class ExternalLoginConfirmationViewModel
	{
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}
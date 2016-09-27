using System;
using System.ComponentModel.DataAnnotations;

namespace NoteFolder.ViewModels {
	public class LogInVM {
		[Required]
		[RegularExpression(@"^[\w-]+$", ErrorMessage = "Usernames must consist only of letters, numbers, hyphens, or underscores.")]
		[Display(Name = "Username")]
		public string UserName { get; set; }
		[Required]
		public string Password { get; set; }
		public string ReturnURL { get; set; }
	}
}
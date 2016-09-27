using System;
using System.ComponentModel.DataAnnotations;

namespace NoteFolder.ViewModels {
	public class LogInVM {
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
		public string ReturnURL { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NoteFolder.ViewModels {
	public class LogInVM {
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
		public string ReturnURL { get; set; }
	}
}
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace NoteFolder.Models {
	public class User : IdentityUser {
		public virtual ICollection<File> Files { get; set; } = new List<File>();
	}
}
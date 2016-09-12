using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NoteFolder.Models {
	public class FileDbContext : DbContext {
		public DbSet<File> Files { get; set; }
	}

	public class File {
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Text { get; set; }
		public bool IsFolder { get; set; }
		public DateTime TimeCreated {get; set; }
		public DateTime TimeLastEdited { get; set; }

		public virtual File Parent { get; set; }
		public virtual ICollection<File> Children { get; set; }
	}
}

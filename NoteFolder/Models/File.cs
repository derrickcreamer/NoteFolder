using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NoteFolder.Models {
	public class FileTestInit : DropCreateDatabaseAlways<FileDbContext> {
		protected override void Seed(FileDbContext db) {
			var docs = new File { Name = "Documents", Description = "", IsFolder = true };
			var proj = new File { Name = "Projects", Description = "", IsFolder = true };
			var nf = new File { Name = "NoteFolder", Description = "Note organization", IsFolder = true };
			var sln = new File { Name = "NoteFolder.sln", Description = "", IsFolder = false, Text = "<Solution text>" };
			var source = new File { Name = "Source", Description = "", IsFolder = true };
			var cs = new File { Name = "NoteFolder.cs", Description = "", IsFolder = false, Text = "<Source code>" };
			proj.SetParent(docs);
			nf.SetParent(proj);
			sln.SetParent(nf);
			source.SetParent(nf);
			cs.SetParent(source);
			db.Files.AddRange(new File[] {docs, proj, nf, sln, source, cs });
			base.Seed(db);
		}
	}
	public static class FileDebugExtensions {
		public static void SetParent(this File f, File parent) {
			f.Parent = parent;
			parent.Children.Add(f);
		}
	}
	public class FileDbContext : DbContext {
		public DbSet<File> Files { get; set; }
		public FileDbContext() {
			Database.SetInitializer(new FileTestInit());
		}
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

using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NoteFolder.Models {
	public class FileTestInit : DropCreateDatabaseAlways<FileDbContext> {
		protected override void Seed(FileDbContext db) {
			var docs = FileModelExtensions.AddFolder(null,"Documents");
			var proj = docs.AddFolder("Projects");
			var nf = proj.AddFolder("NoteFolder", desc: "Note organization");
			var sln = nf.AddNote("NoteFolder.sln", text: "<Solution text here>");
			var source = nf.AddFolder("Source");
			var cs = source.AddNote("NoteFolder.cs", text: "<Source code here>");
			db.Files.AddRange(new File[] {docs, proj, nf, sln, source, cs });
			base.Seed(db);
		}
	}
	public static class FileModelExtensions {
		public static File AddNote(this File parent, string name, string desc = null, string text = null) {
			File f = new File { Parent = parent, Name = name, Description = desc, Text = text, TimeCreated = DateTime.Now };
			f.TimeLastEdited = f.TimeCreated;
			f.InitPath(parent);
			if(parent != null) parent.Children.Add(f);
			return f;
		}
		public static File AddFolder(this File parent, string name, string desc = null) {
			File f = new File { Parent = parent, Name = name, Description = desc, IsFolder = true, TimeCreated = DateTime.Now };
			f.TimeLastEdited = f.TimeCreated;
			f.InitPath(parent);
			if(parent != null) parent.Children.Add(f);
			return f;
		}
		public static void InitPath(this File f, File parent) {
			if(parent == null) f.Path = f.Name;
			else f.Path = parent.Path + "/" + f.Name;
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
		public string Path { get; set; }
		public string Description { get; set; }
		public string Text { get; set; }
		public bool IsFolder { get; set; }
		public DateTime TimeCreated {get; set; }
		public DateTime TimeLastEdited { get; set; }

		public virtual File Parent { get; set; }
		public virtual ICollection<File> Children { get; set; }
	}
}

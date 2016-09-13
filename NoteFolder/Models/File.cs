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
			var notes = nf.AddNote("NoteFolderNotes", text: "<full note text here>");
			var ideas = proj.AddNote("ProjectIdeas", desc: "Only the best!", text: "<full text of project ideas note here>");
			db.Files.AddRange(new File[] {docs, proj, nf, notes, ideas });
			base.Seed(db);
		}
	}
	public static class FileModelExtensions {
		public static string RemoveLastPathSection(this string path) {
			int idx = path.LastIndexOf('/');
			if(idx == -1) return "";
			return path.Substring(0, idx);
		}
		//todo: These extension methods are useful for creating test data, but shouldn't be exposed to the rest of the project.
		public static File AddNote(this File parent, string name, string desc = null, string text = null) {
			File f = new File { Parent = parent, Name = name, Description = desc, Text = text, TimeCreated = DateTime.Now };
			f.TimeLastEdited = f.TimeCreated;
			f.InitPath(parent);
			f.SetParent(parent);
			return f;
		}
		public static File AddFolder(this File parent, string name, string desc = null) {
			File f = new File { Parent = parent, Name = name, Description = desc, IsFolder = true, TimeCreated = DateTime.Now };
			f.TimeLastEdited = f.TimeCreated;
			f.InitPath(parent);
			f.SetParent(parent);
			return f;
		}
		public static void SetParent(this File f, File parent) {
			if(parent != null) {
				if(parent.Children == null) parent.Children = new List<File>();
				parent.Children.Add(f);
			}
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

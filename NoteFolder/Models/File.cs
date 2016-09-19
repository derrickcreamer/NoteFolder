using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
		public static File AddNote(this File parent, string name, string desc = null, string text = null) {
			return new File(parent, false, name, desc, text);
		}
		public static File AddFolder(this File parent, string name, string desc = null) {
			return new File(parent, true, name, desc);
		}
	}
	public class FileDbContext : DbContext {
		public DbSet<File> Files { get; set; }
		public FileDbContext() {
			//Database.SetInitializer(new FileTestInit());
		}

		/// <param name="path">The full path, including separators. Example: "foo/bar/baz". </param>
		public File GetFileByPath(string path) => GetFileByPath(path.Split('/'));
		/// <param name="path">A list of path sections. Example: "foo", "bar", "baz".</param>
		public File GetFileByPath(IList<string> path) {
			if(path.Count == 0) return null;
			string name0 = path[0];
			var query = Files.Where(x => x.ParentID == null && x.Name == name0);
			for(int i=1; i<path.Count; ++i) {
				string nameN = path[i];
				query = query.SelectMany(x => Files.Where(y => y.ParentID == x.ID && y.Name == nameN));
			}
			return query.SingleOrDefault();
		}
	}

	public class File {
		public int ID { get; set; }

		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		public string Text { get; set; }
		public bool IsFolder { get; set; }
		public DateTime TimeCreated { get; set; }
		public DateTime TimeLastEdited { get; set; }
		public int? ParentID { get; set; }

		[InverseProperty("Children")]
		public virtual File Parent { get; set; }

		public virtual ICollection<File> Children { get; set; } = new List<File>();

		public File() { }
		public File(File parent, bool isFolder, string name, string desc = null, string text = null, DateTime? time = null) {
			Parent = parent;
			IsFolder = isFolder;
			Name = name;
			Description = desc;
			Text = text;
			if(time == null) TimeCreated = DateTime.Now;
			else TimeCreated = time.Value;
			TimeLastEdited = TimeCreated;
		}
	}
}

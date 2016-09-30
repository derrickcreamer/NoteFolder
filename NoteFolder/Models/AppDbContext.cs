using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace NoteFolder.Models {
	public class FileInit : DropCreateDatabaseAlways<AppDbContext> {
		protected override void Seed(AppDbContext db) {
			var user = new User { UserName = "guest" };
			var docs = FileModelExtensions.AddFolder(null, "Documents");
			docs.User = user;
			var proj = docs.AddFolder("Projects");
			var nf = proj.AddFolder("NoteFolder", desc: "Note organization");
			var notes = nf.AddNote("NoteFolderNotes", text: "This is the full text\r\nof NoteFolderNotes.");
			var ideas = proj.AddNote("ProjectIdeas", desc: "Only the best!", text: "<full list of ideas here>");
			db.Users.Add(user);
			db.Files.AddRange(new File[] { docs, proj, nf, notes, ideas });
			base.Seed(db);
		}
	}
	public class AppDbContext : IdentityDbContext<User> {
		public DbSet<File> Files { get; set; }
		public AppDbContext() : base("NoteFolder.Models.AppDbContext") {
			Database.SetInitializer(new FileInit());
		}
		public static AppDbContext Create() => new AppDbContext();

		public void DeleteFileRecursively(int id) {
			File file = Files.Find(id);
			if(file != null) DeleteFileRecursively(file);
		}
		public void DeleteFileRecursively(File file) {
			foreach(File child in file.Children.ToList()) {
				DeleteFileRecursively(child);
			}
			Files.Remove(file);
		}

		/// <param name="path">The full path, including separators. Example: "foo/bar/baz". </param>
		public File GetFileByPath(string path) => GetFileByPath(path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
		/// <param name="path">A list of path sections. Example: "foo", "bar", "baz".</param>
		public File GetFileByPath(IList<string> path) {
			if(path.Count == 0) return null;
			string name0 = path[0];
			var query = Files.Where(x => x.ParentID == null && x.Name == name0);
			for(int i = 1;i<path.Count;++i) {
				string nameN = path[i];
				query = query.SelectMany(x => Files.Where(y => y.ParentID == x.ID && y.Name == nameN));
			}
			return query.SingleOrDefault();
		}
	}
}
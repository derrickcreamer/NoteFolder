using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;

namespace NoteFolder.Models {
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

	public static class FileModelExtensions {
		public static File AddNote(this File parent, string name, string desc = null, string text = null) {
			return new File(parent, false, name, desc, text);
		}
		public static File AddFolder(this File parent, string name, string desc = null) {
			return new File(parent, true, name, desc);
		}
	}
}

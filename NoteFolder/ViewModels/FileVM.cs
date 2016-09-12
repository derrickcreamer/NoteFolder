using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NoteFolder.ViewModels {
	public class FileVM {
		public string Name { get; set; }
		public string Path { get; set; }
		public string Description { get; set; }
		public string Text { get; set; }
		public bool IsFolder { get; set; }
		public DateTime TimeCreated { get; set; }
		public DateTime TimeLastEdited { get; set; }
		public ICollection<FileVM> Children { get; set; }
	}
}
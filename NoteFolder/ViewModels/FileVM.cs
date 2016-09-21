using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NoteFolder.ViewModels {
	public class FileVM {
		public int? ExistingID { get; set; }
		[Required]
		public string Name { get; set; }
		public string Path { get; set; }
		public string Description { get; set; }
		public string Text { get; set; }
		public bool IsFolder { get; set; }
		public DateTime TimeCreated { get; set; }
		public DateTime TimeLastEdited { get; set; }
		public int? ParentID { get; set; }
		public ICollection<FileVM> DirectChildren { get; set; }
	}
}
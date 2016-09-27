using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace NoteFolder.ViewModels {
	public class FileVM {
		public int? ExistingID { get; set; }
		[Required]
		[RegularExpression(@"^[\w -]+$", ErrorMessage = "File names must consist only of letters, numbers, spaces, hyphens, or underscores.")]
		[AllowHtml]
		public string Name { get; set; }
		[AllowHtml]
		public string Path { get; set; }
		[AllowHtml]
		public string Description { get; set; }
		[AllowHtml]
		public string Text { get; set; }
		public bool IsFolder { get; set; }
		public bool IsRootFolder { get; set; }
		public DateTime TimeCreated { get; set; }
		public DateTime TimeLastEdited { get; set; }
		public int? ParentID { get; set; }
		public List<FileVM> DirectChildren { get; set; }
	}
	public class DeleteFileVM {
		[Required]
		public int? ExistingID { get; set; }
		[AllowHtml]
		public string Path { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using NoteFolder.Models;
using NoteFolder.ViewModels;
using NoteFolder.Extensions;

namespace NoteFolder.Controllers {
	public class FileController : Controller {
		private FileDbContext db = new FileDbContext();

		/// <summary>
		/// Maps File to FileVM while handling population of DirectChildren collection.
		/// </summary>
		public static FileVM FileVmFromFile(File file, bool includeDirectChildren) {
			FileVM result = Mapper.Map<FileVM>(file);
			result.ExistingID = file.ID;
			if(includeDirectChildren) {
				result.DirectChildren = new List<FileVM>();
				foreach(File child in file.Children) {
					result.DirectChildren.Add(Mapper.Map<FileVM>(child));
				}
			}
			return result;
		}

		public ActionResult Index(string path) {
			if(string.IsNullOrWhiteSpace(path)) {
				FileVM fvm = new FileVM { IsFolder = true, Name = "Files" };
				fvm.DirectChildren = new List<FileVM>();
				foreach(var rootLevelFile in db.Files.Where(x => x.ParentID == null)) {
					fvm.DirectChildren.Add(FileVmFromFile(rootLevelFile, false));
				}
				ViewBag.RootFile = fvm;
				return View(fvm);
			}
			var sections = path.Split('/');
			File file = db.GetFileByPath(sections);
			if(file == null) return new HttpStatusCodeResult(404); //todo, offer to create the missing files?
			else {
				FileVM fvm = FileVmFromFile(file, true);
				fvm.Path = sections;
				return View(fvm);
			}
		}
		//todo: New Note button has inputs for name, desc. isFolder and path might be hidden and included?
		//New Folder has the same.
		[HttpPost]
		public ActionResult Create([Bind(Include = "Name, Path, Description, Text, IsFolder, ParentID")] FileVM f) {
			if(!ModelState.IsValid) {
				return Json(new { success = false, html = this.GetHtmlFromPartialView("_Create", f) });
			}
			if(f.IsFolder) { //todo: Turn this into a validation error, not an exception. (OTOH, it shouldn't happen to normal users.)
				if(f.Text != null) throw new FormatException("Folders cannot have text, only name & description.");
			}
			File dbf = Mapper.Map<File>(f);
			dbf.TimeCreated = DateTime.Now;
			dbf.TimeLastEdited = dbf.TimeCreated;
			db.Files.Add(dbf);
			db.SaveChanges();
			string fullPath = string.Join("/", f.Path) + "/" + f.Name; //Path initially contains the parent's path.
			TempData["LastAction"] = $"{fullPath} created!";
			return Json(new { success = true });
		}
	}
}

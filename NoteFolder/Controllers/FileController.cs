using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NoteFolder.Models;
using NoteFolder.ViewModels;
using AutoMapper;

namespace NoteFolder.Controllers {
	public class FileController : Controller {
		private FileDbContext db = new FileDbContext();

		/// <summary>
		/// Maps File to FileVM while handling population of DirectChildren collection.
		/// </summary>
		public static FileVM FileVmFromFile(File file, bool includeDirectChildren) {
			FileVM result = Mapper.Map<FileVM>(file);
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
		public ActionResult ViewFile(FileVM f) => View(f);
		//todo: New Note button has inputs for name, desc. isFolder and path might be hidden and included?
		//New Folder has the same.
		[HttpPost]
		public ActionResult Create([Bind(Include = "Name, Path, Description, Text, IsFolder")] FileVM f) {
			if(f.IsFolder) {
				if(f.Text != null) throw new FormatException("Folders cannot have text, only name & description.");
			}
			File dbf = Mapper.Map<File>(f);
			dbf.TimeCreated = DateTime.Now;
			dbf.TimeLastEdited = dbf.TimeCreated;
			//todo: temporarily broke parentID assignment here.
			db.Files.Add(dbf);
			db.SaveChanges();
			return RedirectToAction("ViewFile");
		}
	}
}

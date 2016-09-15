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

		public ActionResult Index(string path) {
			if(string.IsNullOrWhiteSpace(path)) {
				FileVM fvm = new FileVM { IsFolder = true, Name = "Files" };
				foreach(var f in db.Files.Where(f => f.Parent == null)) {
					if(fvm.Children == null) fvm.Children = new List<FileVM>();
					fvm.Children.Add(Mapper.Map<FileVM>(f));
				}
				ViewBag.RootFile = fvm;
				return View(fvm);
			}
			File file = db.GetFileByPath(path);
			if(file == null) return new HttpStatusCodeResult(404); //todo, offer to create the missing files?
			else {
				return View(Mapper.Map<FileVM>(file));
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

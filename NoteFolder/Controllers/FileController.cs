using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using AutoMapper;
using NoteFolder.Models;
using NoteFolder.ViewModels;
using NoteFolder.Extensions;

namespace NoteFolder.Controllers {
	public class FileController : Controller {
		private AppDbContext db = new AppDbContext();

		/// <summary>
		/// Maps File to FileVM while handling population of DirectChildren collection.
		/// </summary>
		protected static FileVM FileVmFromFile(File file, bool includeDirectChildren, string path) {
			FileVM result = Mapper.Map<FileVM>(file);
			result.ExistingID = file.ID;
			result.Path = path;
			if(includeDirectChildren) {
				result.DirectChildren = new List<FileVM>();
				foreach(File child in file.Children) {
					FileVM childFVM = Mapper.Map<FileVM>(child);
					childFVM.ExistingID = child.ID;
					childFVM.Path = result.Path + "/" + childFVM.Name;
					result.DirectChildren.Add(childFVM);
				}
				SortFiles(result.DirectChildren);
			}
			return result;
		}
		protected static void SortFiles(List<FileVM> list) {
			list.Sort((a, b) => {
				int typeDiff = -a.IsFolder.CompareTo(b.IsFolder); // Folders first
				if(typeDiff == 0) return string.Compare(a.Name, b.Name, true); // Then sort each group alphabetically
				return typeDiff;
			});
		}
		protected bool VerifyFileAccess(string targetUser, string path) {
			try {
				return string.Compare(targetUser, User.Identity.Name, true) == 0; // Verify same user
			}
			//todo: This method will change if users can ever mark their files 'public' or otherwise share them.
			// Currently, users have access only to their own files.
			// Eventually, this method might check user existence, user role(s), file existence, and file permissions.
			catch {
				throw new InvalidOperationException("Access denied."); // Don't let any exceptions leak information here.
			}
		}

		public ActionResult Index(string user, string path) {
			if(!VerifyFileAccess(user, path)) {
				return RedirectToAction("Index", "File", new { user = User.Identity.Name, path = path });
			}
			if(string.IsNullOrWhiteSpace(path)) {
				FileVM fvm = new FileVM { IsFolder = true, Name = "Files", Path = "" };
				fvm.DirectChildren = new List<FileVM>();
				foreach(var rootLevelFile in db.Files.Where(x => x.ParentID == null)) {
					fvm.DirectChildren.Add(FileVmFromFile(rootLevelFile, false, rootLevelFile.Name));
				}
				SortFiles(fvm.DirectChildren);
				fvm.IsRootFolder = true;
				return View(fvm);
			}
			var sections = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			File file = db.GetFileByPath(sections);
			if(file == null) return new HttpStatusCodeResult(404); //todo, offer to create the missing files?
			else {
				FileVM fvm = FileVmFromFile(file, true, path);
				return View(fvm);
			}
		}
		[HttpPost]
		public ActionResult GetContents(string user, string path) {
			if(!VerifyFileAccess(user, path)) return Json(new { success = false });
			File file = db.GetFileByPath(path);
			if(file == null) return Json(new { success = false });
			FileVM fvm = FileVmFromFile(file, true, path);
			return Json(new { success = true, html = this.GetHtmlFromPartialView("_Contents", fvm) });
		}
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult Create([Bind(Include = "Name, Path, Description, Text, IsFolder, ParentID")] FileVM f) {
			if(!ModelState.IsValid) {
				return Json(new { success = false, html = this.GetHtmlFromPartialView("_Create", f) });
			}
			if(f.IsFolder) { //todo: Turn this into a validation error, not an exception. (OTOH, it shouldn't happen to normal users.)
				if(f.Text != null) throw new FormatException("Folders cannot have text, only name & description.");
			}
			f.Name = f.Name.Trim();
			File existingFile = db.Files.Where(x => x.Name == f.Name).SingleOrDefault();
			if(existingFile != null) {
				ModelState.AddModelError("Name", "A file already exists here with this name.");
				return Json(new { success = false, html = this.GetHtmlFromPartialView("_Create", f) });
			}
			File dbf = Mapper.Map<File>(f);
			dbf.TimeCreated = DateTime.Now;
			dbf.TimeLastEdited = dbf.TimeCreated;
			db.Files.Add(dbf);
			db.SaveChanges();
			string fullPath = null;
			if(string.IsNullOrEmpty(f.Path)) fullPath = f.Name;
			else fullPath = f.Path + "/" + f.Name; //Path initially contains the parent's path.
			TempData["LastAction"] = $"{fullPath} created!";
			return Json(new { success = true, path = fullPath });
		}
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult Edit([Bind(Include = "Name, Path, Description, Text, IsFolder, ExistingID")] FileVM f) {
			if(!ModelState.IsValid) {
				return Json(new { success = false, html = this.GetHtmlFromPartialView("_Edit", f) });
			}
			if(f.IsFolder) { //todo: this should match Create.
				if(f.Text != null) throw new FormatException("Folders cannot have text, only name & description.");
			}
			f.Name = f.Name.Trim();
			File existingFile = db.Files.Where(x => x.Name == f.Name && x.ID != f.ExistingID).SingleOrDefault();
			if(existingFile != null) {
				ModelState.AddModelError("Name", "A file already exists here with this name.");
				return Json(new { success = false, html = this.GetHtmlFromPartialView("_Edit", f) });
			}
			File dbf = db.Files.Find(f.ExistingID);
			dbf.Name = f.Name;
			dbf.Description = f.Description;
			dbf.Text = f.Text;
			dbf.TimeLastEdited = DateTime.Now;
			db.SaveChanges();
			var pathSections = f.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			pathSections[pathSections.Length - 1] = f.Name; //If name changed, update path.
			string fullPath = string.Join("/", pathSections);
			TempData["LastAction"] = $"{fullPath} updated!";
			return Json(new { success = true, path = fullPath });
		}
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult Delete([Bind(Include = "Path, ExistingID")] DeleteFileVM f) {
			if(!ModelState.IsValid) {
				return Json(new { success = false, html = "" }); //todo: This could be improved for failure cases.
			}
			db.DeleteFileRecursively(f.ExistingID.Value);
			db.SaveChanges();
			string parentPath = "";
			int idx = f.Path.LastIndexOf('/');
			if(idx != -1) parentPath = f.Path.Substring(0, idx);
			TempData["LastAction"] = $"{f.Path} deleted!";
			return Json(new { success = true, path = parentPath });
		}
	}
}

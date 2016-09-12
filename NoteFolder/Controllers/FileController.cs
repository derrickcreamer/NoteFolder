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
			File file = db.Files.Where(f => f.Path == path).FirstOrDefault();
			if(file == null) return new HttpStatusCodeResult(404); //todo, offer to create the missing files?
			else {
				return View(Mapper.Map<FileVM>(file));
			}
		}
	}
}

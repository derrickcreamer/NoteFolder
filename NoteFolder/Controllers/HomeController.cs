using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NoteFolder.Models;
using NoteFolder.ViewModels;

namespace NoteFolder.Controllers {
	public class HomeController : Controller {
		[AllowAnonymous]
		public ActionResult Index() => View();
	}
}

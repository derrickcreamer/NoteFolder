using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NoteFolder.Models;

namespace NoteFolder.Controllers {
	public class HomeController : Controller {
		public ActionResult Index() => View();
	}
}

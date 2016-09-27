using System;
using System.Web.Mvc;

namespace NoteFolder.Controllers {
	public class HomeController : Controller {
		[AllowAnonymous]
		public ActionResult Index() => View();
	}
}

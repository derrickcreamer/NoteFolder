using System;
using System.Web.Mvc;

namespace NoteFolder.Extensions {
	public static class ViewExtensions {
		public static string GetHtmlFromPartialView(this Controller controller, string viewName = null, object model = null) {
			if(string.IsNullOrEmpty(viewName)) viewName = controller.ControllerContext.RouteData.GetRequiredString("action");
			controller.ViewData.Model = model;
			using(var writer = new System.IO.StringWriter()) {
				ViewEngineResult vr = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
				ViewContext vc = new ViewContext(controller.ControllerContext, vr.View, controller.ViewData, controller.TempData, writer);
				vr.View.Render(vc, writer);
				return writer.GetStringBuilder().ToString();
			}
		}
	}
}
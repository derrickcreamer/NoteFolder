using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace NoteFolder {
	public class MvcApplication : System.Web.HttpApplication {
		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			ViewModels.FileAutoMapper.Map();
		}
	}
}

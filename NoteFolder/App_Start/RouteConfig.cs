using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NoteFolder
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "File",
				url: "guest/files/{*path}",
				defaults: new { controller = "File", action = "Index", path = "" }
			);

			routes.MapRoute(
                name: "FileAction",
                url: "guest/{action}/files/{*path}",
                defaults: new { controller = "File", action = "Index", path = "" }
            );

			/*routes.MapRoute(
				name: "UserAccount",
				url: "{username}/account",
				defaults: new { controller = "User", action = "Index" }
			);*/

			routes.MapRoute( //todo: I'll want to take another look at routes eventually.
				name: "Home", // (Should thissite.com/foobar try to find a user named foobar, or a Home action?)
				url: "",
				defaults: new { controller = "Home", action = "Index" }
			);

			/*routes.MapRoute( //todo: explicitly include the rest of the Home actions here. There should be only a few.
				name: "About",
				url: "About",
				defaults: new { controller = "Home", action = "About" }
			);*/

			routes.MapRoute(
				name: "Auth",
				url: "{action}",
				defaults: new { controller = "Auth" }
			);
		}
	}
}

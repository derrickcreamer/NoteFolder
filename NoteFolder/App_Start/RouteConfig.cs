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

			routes.MapRoute(
				name: "Home",
				url: "",
				defaults: new { controller = "Home", action = "Index" }
			);
		}
	}
}

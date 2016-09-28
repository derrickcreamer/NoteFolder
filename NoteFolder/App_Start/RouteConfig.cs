using System;
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
				url: "{user}/files/{*path}",
				defaults: new { controller = "File", action = "Index", path = "" }
			);

			routes.MapRoute(
                name: "FileAction",
                url: "{user}/{action}/files/{*path}",
                defaults: new { controller = "File", action = "Index", path = "" }
            );

			routes.MapRoute(
				name: "UserAccount",
				url: "{user}/account",
				defaults: new { controller = "User", action = "Account" }
			);

			routes.MapRoute(
				name: "Home",
				url: "",
				defaults: new { controller = "Home", action = "Index" }
			);

			routes.MapRoute(
				name: "Login",
				url: "login",
				defaults: new { controller = "User", action = "Login" }
			);

			routes.MapRoute(
				name: "Logout",
				url: "logout",
				defaults: new { controller = "User", action = "Logout" } // POST only
			);

			routes.MapRoute(
				name: "Register",
				url: "register",
				defaults: new { controller = "User", action = "Register" }
			);
			/*routes.MapRoute( //todo: explicitly include the rest of the Home & Auth actions here. There should be only a few.
				name: "About",
				url: "About",
				defaults: new { controller = "Home", action = "About" }
			);*/

			routes.MapRoute(
				name: "UserShorthand",
				url: "{user}",
				defaults: new { controller = "User", action = "ShorthandRedirect" }
			);
		}
	}
}

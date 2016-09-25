using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;

namespace NoteFolder {
	public partial class Startup {
		public void Configuration(IAppBuilder app) {
			app.UseCookieAuthentication(new CookieAuthenticationOptions {
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Home/Login")
			});
		}
	}
}
using System;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using NoteFolder.Models;

namespace NoteFolder {
	public partial class Startup {
		public void Configuration(IAppBuilder app) {
			app.CreatePerOwinContext<AppDbContext>(AppDbContext.Create);
			app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
			app.UseCookieAuthentication(new CookieAuthenticationOptions {
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Login"),
				ExpireTimeSpan = TimeSpan.FromDays(7),
				Provider = new CookieAuthenticationProvider {
					OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager, User>
					(
						validateInterval: TimeSpan.FromSeconds(30),
						regenerateIdentity:
							(manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie)
					)
				}
			});
		}
	}
}
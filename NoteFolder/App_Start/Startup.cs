using System;
using System.Web.Script.Serialization;
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
				ExpireTimeSpan = TimeSpan.FromDays(999),
				Provider = new CookieAuthenticationProvider {
					OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager, User>
					(
						validateInterval: TimeSpan.FromSeconds(30),
						regenerateIdentity:
							(manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie)
					),
					OnApplyRedirect = ctx => {
						if(!IsAjaxRequest(ctx.Request)) ctx.Response.Redirect(ctx.RedirectUri);
						else {
							var jsonObj = new { // Return json for ajax auth failures. If extra data needs to be sent,
								authFailed = true, // it can easily be done here.
							};
							string json = new JavaScriptSerializer().Serialize(jsonObj);
							ctx.Response.StatusCode = 200;
							ctx.Response.ContentType = "application/json";
							ctx.Response.Write(json);
						}
					}
				}
			});
		}
		private static bool IsAjaxRequest(IOwinRequest req) {
			return req.Query?["X-Requested-With"] == "XMLHttpRequest" || req.Headers?["X-Requested-With"] == "XMLHttpRequest";
		}
	}
}
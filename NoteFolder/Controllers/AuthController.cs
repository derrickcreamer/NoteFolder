using System;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NoteFolder.Models;
using NoteFolder.ViewModels;

namespace NoteFolder.Controllers {
	public class AuthController : Controller {
		private UserManager<IdentityUser> UserManager;
		public AuthController() {
			UserManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new AppDbContext()));
			UserManager.UserValidator = new UserValidator<IdentityUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
		}

		[AllowAnonymous]
		public ActionResult LogIn(string returnURL) => View(new LogInVM { ReturnURL = returnURL });

		[AllowAnonymous]
		[HttpPost]
		public ActionResult LogIn(LogInVM login) {
			if(!ModelState.IsValid) return View(login);
			var user = UserManager.Find(login.Email, login.Password);
			if(user == null) {
				ModelState.AddModelError("", "Invalid email or password. Please try again.");
				return View(login);
			}
			var idCookie = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
			Request.GetOwinContext().Authentication.SignIn(idCookie);
			return RedirectToLocal(login.ReturnURL); //todo: Perhaps this should redirect to user page instead.
		}
		protected ActionResult RedirectToLocal(string returnURL) {
			if(string.IsNullOrWhiteSpace(returnURL) || !Url.IsLocalUrl(returnURL)) return RedirectToAction("Index", "Home");
			else return Redirect(returnURL);
		}
		[HttpPost]
		public ActionResult LogOut() {
			Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			return RedirectToAction("Index", "Home");
		}
	}
}
using System;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using NoteFolder.Models;
using NoteFolder.ViewModels;

namespace NoteFolder.Controllers {
	public class UserController : Controller {
		private AppUserManager UserManager => HttpContext.GetOwinContext().GetUserManager<AppUserManager>();

		[AllowAnonymous]
		public ActionResult LogIn(string returnURL) {
			if(User.Identity.IsAuthenticated) return RedirectToLocal(returnURL); //todo: Could redirect to user account page.
			else return View(new LogInVM { ReturnURL = returnURL });
		}

		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult LogIn(LogInVM login) {
			if(!ModelState.IsValid) return View(login);
			var user = UserManager.Find(login.UserName, login.Password);
			if(user == null) {
				ModelState.AddModelError("", "Invalid username or password. Please try again.");
				return View(login);
			}
			SignIn(user);
			return RedirectToLocal(login.ReturnURL); //todo: Perhaps this should redirect to user page instead.
		}
		protected ActionResult RedirectToLocal(string returnURL) {
			if(string.IsNullOrWhiteSpace(returnURL) || !Url.IsLocalUrl(returnURL)) return RedirectToAction("Index", "Home");
			else return Redirect(returnURL);
		}
		protected void SignIn(User user) {
			var idCookie = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
			Request.GetOwinContext().Authentication.SignIn(idCookie);
		}

		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult LogOut() {
			Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			UserManager.UpdateSecurityStamp(User.Identity.GetUserId());
			TempData["LastAction"] = "You have successfully logged out.";
			return RedirectToAction("Index", "Home");
		}

		[AllowAnonymous]
		public ActionResult Register(string returnURL) {
			if(User.Identity.IsAuthenticated) return RedirectToLocal(returnURL); //todo: same as Login above - user page?
			else return View(new LogInVM { ReturnURL = returnURL });
		}

		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[HttpPost]
		public ActionResult Register(LogInVM register) { //todo: Take another look at whether register & login VMs are the same.
			if(!ModelState.IsValid) return View(register);
			var user = new User { UserName = register.UserName };
			var result = UserManager.Create(user, register.Password);
			if(!result.Succeeded) {
				foreach(var error in result.Errors) ModelState.AddModelError("", error);
				return View(register);
			}
			SignIn(user);
			TempData["LastAction"] = $"Account created! Welcome, {register.UserName}.";
			return RedirectToLocal(register.ReturnURL);
		}

		public ActionResult ShorthandRedirect(string user) => RedirectToActionPermanent("Index", "File", new { user = user });
	}
}
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
			SignIn(user);
			return RedirectToLocal(login.ReturnURL); //todo: Perhaps this should redirect to user page instead.
		}
		protected ActionResult RedirectToLocal(string returnURL) {
			if(string.IsNullOrWhiteSpace(returnURL) || !Url.IsLocalUrl(returnURL)) return RedirectToAction("Index", "Home");
			else return Redirect(returnURL);
		}
		protected void SignIn(IdentityUser user) {
			var idCookie = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
			Request.GetOwinContext().Authentication.SignIn(idCookie);
		}

		[HttpPost]
		public ActionResult LogOut() {
			Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			TempData["LastAction"] = "You have successfully logged out.";
			return RedirectToAction("Index", "Home");
		}

		[AllowAnonymous]
		public ActionResult Register(string returnURL) => View(new LogInVM { ReturnURL = returnURL });

		[AllowAnonymous]
		[HttpPost]
		public ActionResult Register(LogInVM register) { //todo: Take another look at whether register & login VMs are the same.
			if(!ModelState.IsValid) return View(register);
			var user = new IdentityUser { UserName = register.Email };
			var result = UserManager.Create(user, register.Password);
			if(!result.Succeeded) {
				foreach(var error in result.Errors) ModelState.AddModelError("", error);
				return View(register);
			}
			SignIn(user);
			TempData["LastAction"] = "Account created!";
			return RedirectToLocal(register.ReturnURL);
		}
	}
}
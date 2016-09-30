using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;

namespace NoteFolder.Models {
	public class AppUserManager : UserManager<User> {
		public AppUserManager(IUserStore<User> store) : base(store) { }
		public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext ctx) {
			var result = new AppUserManager(new UserStore<User>(ctx.Get<AppDbContext>()));
			result.UserValidator = new UserValidator<User>(result) { AllowOnlyAlphanumericUserNames = false };
			return result;
		}
	}
}
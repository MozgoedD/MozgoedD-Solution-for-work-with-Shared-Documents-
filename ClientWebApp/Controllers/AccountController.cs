using ClientWebApp.Infrastructure;
using ClientWebApp.Models;
using ClientWebApp.Services.Abstract;
using ClientWebApp.Services.Concrete;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace ClientWebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    SecondName = model.SecondName,
                    Patronymic = model.Patronymic,
                    Gender = model.Gender,
                    DOB = model.DOB,
                    Workplace = model.Workplace,
                    JobPosition = model.JobPosition,
                    City = model.City,
                    Country = model.Country,
                    IsApproved = false,
                    RawPassword = System.Web.Security.Membership.GeneratePassword(5, 0)
                };

                var result = await UserManager.CreateAsync(user, user.RawPassword);

                if (result.Succeeded)
                {
                    result = await UserManager.AddToRoleAsync(user.Id, "Users");
                    if (!result.Succeeded)
                    {
                        AddErrorsFromResult(result);
                    }
                    else
                    {
                        user.SharePointId = spUserManager.AddUserToSpList(user);
                        result = await UserManager.UpdateAsync(user);
                        if (!result.Succeeded)
                        {
                            AddErrorsFromResult(result);
                        }
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginNotAccess");
            }

            ViewBag.returnUrl = returnUrl;
            return View();
        }

        public ActionResult LoginNotAccess(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel details, string returnUrl)
        {
            var user = await UserManager.FindAsync(details.Name, details.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Username or Password");
            }
            else
            {
                if (user.IsApproved)
                {
                    ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

                    AuthManager.SignOut();
                    AuthManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, ident);

                    if (returnUrl == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(details);
        }

        [Authorize]
        public ActionResult Logout()
        {
            AuthManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ISharePointUserManagerService spUserManager
        {
            get
            {
                return new SharePointUserManagerService();
            }
        }

        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
    }
}
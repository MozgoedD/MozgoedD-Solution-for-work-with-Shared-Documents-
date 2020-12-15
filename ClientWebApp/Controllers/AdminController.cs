﻿using ClientWebApp.Infrastructure;
using ClientWebApp.Models;
using ClientWebApp.Services.Abstract;
using ClientWebApp.Services.Concrete;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace ClientWebApp.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View(UserManager.Users);
        }

        public async Task<ActionResult> ApprovePage(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ApprovePage(string id, bool isApproved)
        {
            if (isApproved)
            {
                return RedirectToAction("Index");
            }
            else
            {
                AppUser user = await UserManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.IsApproved = true;
                    var result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        spUserManager.ApproveUser(user);
                        //EmailManager.Send("Your account is now approved!",
                        //    $"Your account is now approved!\nYour password is: {user.RawPassword}", user.Email);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User Not Found");
                }
                return View(user);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Reject(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                var userMail = user.Email;
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    spUserManager.DeleteUser(user);
                    //EmailManager.Send("Your account is not approved!",
                    //    $"Your account is not approved! Please try to create a new request", userMail);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "User not found" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                var userMail = user.Email;
                var result = await UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    spUserManager.DeleteUser(user);
                    //EmailManager.Send("Your account has been deleted!",
                    //    $"Your account has been deleted! Please try to create a new request", userMail);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", result.Errors);
                }
            }
            else
            {
                return View("Error", new string[] { "User not found" });
            }
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

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }
    }
}
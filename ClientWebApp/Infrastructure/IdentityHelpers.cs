using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientWebApp.Infrastructure
{
    public static class IndentityHelpers
    {
        public static MvcHtmlString GetUserName(this HtmlHelper html, string id)
        {
            if (id == "SharePointAuthorId")
            {
                return new MvcHtmlString("SharePoint Author");
            }

            AppUserManager UserManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();

            try
            {
                MvcHtmlString result = new MvcHtmlString(UserManager.FindByIdAsync(id).Result.UserName);
            }

            catch (System.NullReferenceException)
            {
                return new MvcHtmlString("User No Exist");
            }

            return new MvcHtmlString(UserManager.FindByIdAsync(id).Result.UserName);
        }

        //public static MvcHtmlString GetCurrentUserName(this HtmlHelper html)
        //{
        //    AppUserManager UserManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();

        //    MvcHtmlString userName = new MvcHtmlString(UserManager.FindByName(HttpContext.User.Identity.Name));

        //    return userName;
        //}
    }
}
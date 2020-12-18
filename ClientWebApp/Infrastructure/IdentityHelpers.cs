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
            var UserManager = HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            try
            {
                var result = new MvcHtmlString(UserManager.FindByIdAsync(id).Result.UserName);
                return result;
            }
            catch (NullReferenceException)
            {
                return new MvcHtmlString("User No Exist");
            }
        }
    }
}
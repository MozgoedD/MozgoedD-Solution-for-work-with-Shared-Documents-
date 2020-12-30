using Core.Models;
using DAL.Abstract;
using DAL.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DAL.Concrete
{
    public class IdentityManager : IIdentityManager
    {
        public IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        public AppUserManager UserManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        public AppUser CurrentUser
        {
            get
            {
                return UserManager.FindByName(HttpContext.Current.User.Identity.Name);
            }
        }
    }
}

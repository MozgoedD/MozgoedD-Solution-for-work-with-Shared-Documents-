using Core.Models;
using DAL.Infrastructure;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Abstract
{
    public interface IIdentityManager
    {
        AppUserManager UserManager { get; }
        IAuthenticationManager AuthManager { get; }
        AppUser CurrentUser { get; }
    }
}

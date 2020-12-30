using BLL.Abstract;
using Core.Models;
using DAL.Abstract;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Concrete
{
    public class LoginService : ILoginService
    {
        ISharePointUserService _spUserManager;
        IIdentityManager _identityManager;

        public LoginService(ISharePointUserService spUserManager, IIdentityManager identityManager)
        {
            _spUserManager = spUserManager;
            _identityManager = identityManager;
        }

        public async Task<string> CreateUser(AppUser userToCreate)
        {
            var result = await _identityManager.UserManager.CreateAsync(userToCreate, userToCreate.RawPassword);

            if (result.Succeeded)
            {
                result = await _identityManager.UserManager.AddToRoleAsync(userToCreate.Id, "Users");
                if (!result.Succeeded)
                {
                    return string.Join(", ", result.Errors.ToArray());
                }
                else
                {
                    userToCreate.SharePointId = _spUserManager.AddUserToSpList(userToCreate);
                    result = await _identityManager.UserManager.UpdateAsync(userToCreate);
                    if (!result.Succeeded)
                    {
                        return string.Join(", ", result.Errors.ToArray());
                    }
                    return null;
                }
            }
            else
            {
                return string.Join(", ", result.Errors.ToArray());
            }
        }

        public async Task<AppUser> GetUserByCreds(string userName, string userPassword)
        {
            return await _identityManager.UserManager.FindAsync(userName, userPassword);
        }

        public async Task Login(AppUser user, string applicationCookie)
        {
            var ident = await _identityManager.UserManager.CreateIdentityAsync(user, applicationCookie);

            _identityManager.AuthManager.SignOut();
            _identityManager.AuthManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
            }, ident);
        }

        public void Logout()
        {
            _identityManager.AuthManager.SignOut();
        }
    }
}

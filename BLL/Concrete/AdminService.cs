using BLL.Abstract;
using Core.Models;
using DAL.Abstract;
using DAL.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLL.Concrete
{
    public class AdminService : IAdminService
    {
        IEmailService _emailManager;
        ISharePointUserService _spUserManager;
        IIdentityManager _identityManager;

        public AdminService(IEmailService emailManager,
            ISharePointUserService spUserManager,
            IIdentityManager identityManager)
        {
            _emailManager = emailManager;
            _spUserManager = spUserManager;
            _identityManager = identityManager;
        }

        public async Task<string> ApproveUser(string id)
        {
            var user = await _identityManager.UserManager.FindByIdAsync(id);
            if (user != null)
            {
                user.IsApproved = true;
                var result = await _identityManager.UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _spUserManager.ApproveUser(user);
                    _emailManager.SendEmail("Your account is now approved!",
                        $"Your account is now approved!\nYour password is: {user.RawPassword}", user.Email);
                    return null;
                }
                else return string.Join(", ", result.Errors.ToArray());
            }
            else return "User Not Found";
        }

        public async Task<string> DeleteUser(string id)
        {
            var user = await _identityManager.UserManager.FindByIdAsync(id);
            if (user != null)
            {
                var userMail = user.Email;
                var result = await _identityManager.UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _spUserManager.DeleteUser(user);
                    _emailManager.SendEmail("Your account has been deleted!",
                        "Your account has been deleted! Please try to create a new request", userMail);
                    return null;
                }
                else return string.Join(", ", result.Errors.ToArray());
            }
            else return "User not found";
        }

        public IQueryable<AppUser> GetAll()
        {
            return _identityManager.UserManager.Users;
        }

        public async Task<AppUser> GetById(string id)
        {
            var user = await _identityManager.UserManager.FindByIdAsync(id);
            return user;
        }

        public async Task<string> RejectUser(string id)
        {
            var user = await _identityManager.UserManager.FindByIdAsync(id);
            if (user != null)
            {
                var userMail = user.Email;
                var result = await _identityManager.UserManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    _spUserManager.DeleteUser(user);
                    _emailManager.SendEmail("Your account is not approved!",
                        "Your account is not approved! Please try to create a new request", userMail);
                    return null;
                }
                else return string.Join(", ", result.Errors.ToArray());
            }
            else return "User not found";
        }
    }
}

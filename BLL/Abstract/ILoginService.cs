using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstract
{
    public interface ILoginService
    {
        Task<string> CreateUser(AppUser userToCreate);
        Task<AppUser> GetUserByCreds(string userName, string userPassword);
        Task Login(AppUser user, string applicationCookie);
        void Logout();
    }
}

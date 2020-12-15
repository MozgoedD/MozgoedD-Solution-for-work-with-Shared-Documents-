using ClientWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWebApp.Services.Abstract
{
    public interface ISharePointUserManagerService
    {
        int AddUserToSpList(AppUser user);
        void ApproveUser(AppUser user);
        void DeleteUser(AppUser user);
    }
}

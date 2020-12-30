using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Abstract
{
    public interface ISharePointUserService
    {
        int AddUserToSpList(AppUser user);
        void ApproveUser(AppUser user);
        void DeleteUser(AppUser user);
    }
}

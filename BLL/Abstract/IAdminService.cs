using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstract
{
    public interface IAdminService
    {
        IQueryable<AppUser> GetAll();
        Task<AppUser> GetById(string id);
        Task<string> ApproveUser(string id);
        Task<string> RejectUser(string id);
        Task<string> DeleteUser(string id);
    }
}

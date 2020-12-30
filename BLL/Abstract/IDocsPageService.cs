using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLL.Abstract
{
    public interface IDocsPageService
    {
        AppUser GetCurrentUser();
        IEnumerable<AppFileModel> GetAllFiles();
        int UploadFile(AppFileModel fileToUpload, HttpPostedFileBase upFile);
        AppFileModel GetFileById(int id);
        int DeleteFile(int id);
        int UserMenu();

    }
}

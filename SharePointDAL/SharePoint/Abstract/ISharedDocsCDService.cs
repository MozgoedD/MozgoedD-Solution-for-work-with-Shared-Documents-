using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.SharePoint.Abstract
{
    public interface ISharedDocsCDService
    {
        void UploadFileToSP(AppFileModel file);
        void DeleteFileFromSP(AppFileModel file);
    }
}

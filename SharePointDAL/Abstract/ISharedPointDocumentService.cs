using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Abstract
{
    public interface ISharedPointDocumentService
    {
        List<AppFileModel> getAppFileModelObject();
        void UploadFileToSP(AppFileModel file);
        void DeleteFileFromSP(AppFileModel file);
    }
}

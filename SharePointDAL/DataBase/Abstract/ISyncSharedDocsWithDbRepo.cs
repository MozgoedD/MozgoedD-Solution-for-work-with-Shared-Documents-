using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointDAL.DataBase.Abstract
{
    public interface ISyncSharedDocsWithDbRepo
    {
        void UpdateFilesInDb(List<AppFileModel> filesInSp);
    }
}

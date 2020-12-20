using ConsoleSyncApp.Models;
using SharePointDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSyncApp.Services.Abstract
{
    public interface ISyncWithDbRepo
    {
        void UpdateFilesInDb(List<AppFileModel> filesInSp);
    }
}

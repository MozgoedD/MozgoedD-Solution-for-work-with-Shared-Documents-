using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Abstract
{
    public interface IFileRepository : IRepository<AppFileModel>
    {
        //void UpdateFilesInDb(List<AppFileModel> filesInSp);
    }
}

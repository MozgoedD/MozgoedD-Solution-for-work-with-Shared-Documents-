using SharePointDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointDAL.Abstract
{
    public interface ISharedDocsGetter
    {
        List<AppFileModel> getAppFileModelObject();
    }
}

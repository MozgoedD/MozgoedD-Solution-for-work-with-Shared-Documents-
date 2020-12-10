using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleSyncApp.Models
{
    public class AppFileModel
    {
        public int Id { get; set; }
        public string AuthorId { get; set; }
        public string Name { get; set; }
        public byte[] File { get; set; }
        public int SharePointId { get; set; }
    }

    //public static class FileComparer()
    //{
    //    public static bool CompareFiles(AppFileModel a, AppFileModel b)
    //    {
    //        FileStream fs1 = new FileStream(a.File.)
    //    }
    //}

    //public class FileComparer : IEqualityComparer<AppFileModel>
    //{
    //    public bool Equals(AppFileModel x, AppFileModel y)
    //    {
    //        if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null)) return false;
    //        else if (x.Name == y.Name) return true;
    //        else return false;
    //    }

    //    public int GetHashCode(AppFileModel obj)
    //    {
    //        int nameHash = obj.Name.GetHashCode();
    //        int fileHash = obj.File.GetHashCode();

    //        return nameHash ^ fileHash;
    //    }
    //}
}

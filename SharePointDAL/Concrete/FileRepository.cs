using Core.Models;
using DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Concrete
{
    public class FileRepository : Repository<AppFileModel>, IFileRepository
    {
        public FileRepository(DbContext dbContext)
            : base(dbContext) { }
    }
}

        //public void UpdateFilesInDb(List<AppFileModel> filesInSp)
        //{
        //    var filesInDB = _dbContext.Files.ToList();

        //    // Checking for deleted files
        //    foreach (AppFileModel fileInDB in filesInDB)
        //    {
        //        if (!filesInSp.Exists(f => f.Name == fileInDB.Name))
        //        {
        //            AppFileModel fileDB = _dbContext.Files.Find(fileInDB.Id);
        //            _dbContext.Files.Remove(fileDB);
        //            _dbContext.SaveChanges();
        //            Console.WriteLine($"{fileInDB.Name} has been deleted from mvc app database!");
        //        }
        //    }

        //    // Checking for added files
        //    foreach (AppFileModel fileInSP in filesInSp)
        //    {
        //        if (!filesInDB.Exists(f => f.Name == fileInSP.Name))
        //        {
        //            _dbContext.Files.Add(fileInSP);
        //            _dbContext.SaveChanges();
        //            Console.WriteLine($"{fileInSP.Name} has been added to mvc app database!");
        //        }

        //        // Checking for update
        //        else
        //        {
        //            AppFileModel fileToUpdate = filesInDB.Where(f => f.Name == fileInSP.Name).FirstOrDefault();
        //            if (!fileToUpdate.File.SequenceEqual(fileInSP.File))
        //            {
        //                _dbContext.Files.Remove(fileToUpdate);
        //                _dbContext.Files.Add(fileInSP);
        //                _dbContext.SaveChanges();
        //                Console.WriteLine($"{fileInSP.Name} has been updated in mvc app database!");
        //            }
        //        }
        //    }
//        }
//    }
//}

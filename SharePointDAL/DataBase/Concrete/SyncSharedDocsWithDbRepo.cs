using DAL.Models;
using SharePointDAL.DataBase.Abstract;
using SharePointDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointDAL.DataBase.Concrete
{
    public class SyncSharedDocsWithDbRepo : ISyncSharedDocsWithDbRepo
    {
        public void UpdateFilesInDb(List<AppFileModel> filesInSP)
        {
            using (AppDbContext filesContext = new AppDbContext())
            {
                var filesInDB = filesContext.Files.ToList();

                // Checking for deleted files
                foreach (AppFileModel fileInDB in filesInDB)
                {
                    if (!filesInSP.Exists(f => f.Name == fileInDB.Name))
                    {
                        AppFileModel fileDB = filesContext.Files.Find(fileInDB.Id);
                        filesContext.Files.Remove(fileDB);
                        filesContext.SaveChanges();
                        Console.WriteLine($"{fileInDB.Name} has been deleted from mvc app database!");
                    }
                }

                // Checking for added files
                foreach (AppFileModel fileInSP in filesInSP)
                {
                    if (!filesInDB.Exists(f => f.Name == fileInSP.Name))
                    {
                        filesContext.Files.Add(fileInSP);
                        filesContext.SaveChanges();
                        Console.WriteLine($"{fileInSP.Name} has been added to mvc app database!");
                    }

                    // Checking for update
                    else
                    {
                        AppFileModel fileToUpdate = filesInDB.Where(f => f.Name == fileInSP.Name).FirstOrDefault();
                        if (!fileToUpdate.File.SequenceEqual(fileInSP.File))
                        {
                            filesContext.Files.Remove(fileToUpdate);
                            filesContext.Files.Add(fileInSP);
                            filesContext.SaveChanges();
                            Console.WriteLine($"{fileInSP.Name} has been updated in mvc app database!");
                        }
                    }
                }
            }
        }
    }
}

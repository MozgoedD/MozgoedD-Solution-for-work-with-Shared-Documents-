using BLL.Abstract;
using Core.Models;
using DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Concrete
{
    public class SyncSharedDocsWithDbRepo : ISyncSharedDocsWithDbRepo
    {
        IUnitOfWork _unitOfWork;

        public SyncSharedDocsWithDbRepo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void UpdateFilesInDb(List<AppFileModel> filesInSP)
        {
            var filesInDB = _unitOfWork.Files.GetAll();

            // Checking for deleted files
            foreach (AppFileModel fileInDB in filesInDB)
            {
                if (!filesInSP.Exists(f => f.Name == fileInDB.Name))
                {
                    var fileInDbToDelete = _unitOfWork.Files.Get(fileInDB.Id);
                    _unitOfWork.Files.Remove(fileInDbToDelete);
                    Console.WriteLine($"{fileInDB.Name} has been deleted from mvc app database!");
                }
            }
            _unitOfWork.Complete();

            // Checking for added files
            foreach (AppFileModel fileInSP in filesInSP)
            {
                if (!filesInDB.Any(f => f.Name == fileInSP.Name))
                {
                    _unitOfWork.Files.Create(fileInSP);
                    Console.WriteLine($"{fileInSP.Name} has been added to mvc app database!");
                }

                // Checking for update
                else
                {
                    AppFileModel fileToUpdate = filesInDB.Where(f => f.Name == fileInSP.Name).FirstOrDefault();
                    if (!fileToUpdate.File.SequenceEqual(fileInSP.File))
                    {
                        _unitOfWork.Files.Remove(fileToUpdate);
                        _unitOfWork.Files.Create(fileInSP);
                        Console.WriteLine($"{fileInSP.Name} has been updated in mvc app database!");
                    }
                }
            }
            _unitOfWork.Complete();
        }
    }
}

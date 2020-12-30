using BLL.Abstract;
using Core.Models;
using DAL.Abstract;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLL.Concrete
{
    public class DocsPageService : IDocsPageService
    {
        IIdentityManager _identityManager;
        IUnitOfWork _unitOfWork;
        ISharedPointDocumentService _spFileManager;

        public DocsPageService(IIdentityManager identityManager, IUnitOfWork unitOfWork,
            ISharedPointDocumentService spFileManager)
        {
            _identityManager = identityManager;
            _unitOfWork = unitOfWork;
            _spFileManager = spFileManager;
        }

        public AppUser GetCurrentUser()
        {
            return _identityManager.CurrentUser;
        }

        public IEnumerable<AppFileModel> GetAllFiles()
        {
            return _unitOfWork.Files.GetAll();
        }

        public int UploadFile(AppFileModel fileToUpload, HttpPostedFileBase upFile)
        {
            var fileInDb = _unitOfWork.Files.Find(f => f.Name == fileToUpload.Name).FirstOrDefault();
            if (fileInDb == null)
            {
                UploadFileProcedure(fileToUpload, upFile);
                return 0;
            }
            else
            {
                var user = _identityManager.CurrentUser;
                if (user.Id == fileInDb.AuthorId || _identityManager.UserManager.IsInRole(user.Id, "Administrators"))
                {
                    _spFileManager.DeleteFileFromSP(fileInDb);
                    _unitOfWork.Files.Remove(fileInDb);
                    _unitOfWork.Complete();

                    UploadFileProcedure(fileToUpload, upFile);
                    return 0;
                }
                else return 1;
            }
        }

        private void UploadFileProcedure(AppFileModel fileToUpload, HttpPostedFileBase upFile)
        {
            var upFileByteArr = new byte[upFile.InputStream.Length];
            upFile.InputStream.Read(upFileByteArr, 0, upFileByteArr.Length);
            fileToUpload.File = upFileByteArr.ToArray();

            _spFileManager.UploadFileToSP(fileToUpload);

            _unitOfWork.Files.Create(fileToUpload);
            _unitOfWork.Complete();
        }

        public AppFileModel GetFileById(int id)
        {
            return _unitOfWork.Files.Get(id);
        }

        public int DeleteFile(int id)
        {
            var fileInDb = _unitOfWork.Files.Get(id);
            if (fileInDb == null) return 2;

            var user = _identityManager.CurrentUser;
            if (user.Id == fileInDb.AuthorId || _identityManager.UserManager.IsInRole(user.Id, "Administrators"))
            {
                _spFileManager.DeleteFileFromSP(fileInDb);
                _unitOfWork.Files.Remove(fileInDb);
                _unitOfWork.Complete();

                return 0;
            }
            else return 1;
        }

        public int UserMenu()
        {
            var user = _identityManager.CurrentUser;
            if (user != null)
            {
                if (_identityManager.UserManager.IsInRole(user.Id, "Administrators"))
                {
                    return 1;
                }
                return 0;
            }
            else
            {
                return 2;
            }
        }
    }
}

using ClientWebApp.Infrastructure;
using ClientWebApp.Models;
using ClientWebApp.Services.Abstract;
using ClientWebApp.Services.Concrete;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ClientWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        AppDbContext filesContext = new AppDbContext();

        public ActionResult Index()
        {
            var user = CurrentUser;
            if (user != null)
            {
                ViewBag.UserName = user.UserName;
            }
            return View(filesContext.Files);
        }

        public ActionResult Create()
        {
            var user = CurrentUser;
            ViewBag.AuthorId = user.Id;
            return View();
        }

        [HttpPost]
        public ActionResult Create(FileCreateModel fileModel)
        {
            if (ModelState.IsValid)
            {
                AppFileModel file = new AppFileModel
                {
                    Name = Path.GetFileName(fileModel.File.FileName),
                    AuthorId = fileModel.AuthorId,
                };

                var fileInDb = filesContext.Files.Where(f => f.Name == file.Name).FirstOrDefault();
                if (fileInDb == null)
                {
                    var uploadedFile = new byte[fileModel.File.InputStream.Length];
                    fileModel.File.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                    file.File = uploadedFile.ToArray();
                    spDocsManager.UploadFileToSP(file);

                    filesContext.Files.Add(file);
                    filesContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var user = CurrentUser;
                    if (user.Id == fileInDb.AuthorId || UserManager.IsInRole(user.Id, "Administrators"))
                    {
                        spDocsManager.DeleteFileFromSP(fileInDb);
                        filesContext.Files.Remove(fileInDb);
                        filesContext.SaveChanges();

                        var uploadedFile = new byte[fileModel.File.InputStream.Length];
                        fileModel.File.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                        file.File = uploadedFile.ToArray();
                        spDocsManager.UploadFileToSP(file);

                        filesContext.Files.Add(file);
                        filesContext.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("LoginNotAccess", "Account");
                    }
                }
            }
            return View();
        }

        public FileResult Download(int fileId)
        {
            var file = filesContext.Files.Find(fileId);
            return File(file.File, "multipart/form-data", file.Name);
        }

        [HttpPost]
        public ActionResult Delete(int fileId)
        {
            var user = CurrentUser;
            var file = filesContext.Files.Find(fileId);
            if (file == null)
            {
                return View("Error", new string[] { "User not found" });
            }
            else
            {
                if (user.Id == file.AuthorId || UserManager.IsInRole(user.Id, "Administrators"))
                {
                    spDocsManager.DeleteFileFromSP(file);
                    filesContext.Files.Remove(file);
                    filesContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("LoginNotAccess", "Account");
                }
            }
        }

        public ActionResult About()
        {
            var user = CurrentUser;
            if (user != null)
            {
                if (UserManager.IsInRole(user.Id, "Administrators"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        private ISharePointSharedDocsManagerService spDocsManager
        {
            get
            {
                return new SharePointSharedDocsManagerService();
            }
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        private AppUser CurrentUser
        {
            get
            {
                return UserManager.FindByName(HttpContext.User.Identity.Name);
            }
        }
    }
}
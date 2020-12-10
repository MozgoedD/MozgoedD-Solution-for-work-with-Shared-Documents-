using ClientWebApp.Infrastructure;
using ClientWebApp.Models;
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
            AppUser user = CurrentUser;
            if (user != null)
            {
                ViewBag.UserName = user.UserName;
            }
            return View(filesContext.Files);
        }

        public ActionResult Create()
        {
            AppUser user = CurrentUser;
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
                    byte[] uploadedFile = new byte[fileModel.File.InputStream.Length];
                    fileModel.File.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                    file.File = uploadedFile.ToArray();
                    SharePointManager.UploadFileToSP(file);

                    filesContext.Files.Add(file);
                    filesContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    AppUser user = CurrentUser;
                    if (user.Id == fileInDb.AuthorId || UserManager.IsInRole(user.Id, "Administrators"))
                    {
                        SharePointManager.DeleteFileFromSP(fileInDb);
                        filesContext.Files.Remove(fileInDb);
                        filesContext.SaveChanges();

                        byte[] uploadedFile = new byte[fileModel.File.InputStream.Length];
                        fileModel.File.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                        file.File = uploadedFile.ToArray();
                        SharePointManager.UploadFileToSP(file);

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
            AppFileModel file = filesContext.Files.Find(fileId);
            return File(file.File, "multipart/form-data", file.Name);
        }

        [HttpPost]
        public ActionResult Delete(int fileId)
        {
            AppUser user = CurrentUser;
            AppFileModel file = filesContext.Files.Find(fileId);
            if (file == null)
            {
                return View("Error", new string[] { "User not found" });
            }
            else
            {
                if (user.Id == file.AuthorId || UserManager.IsInRole(user.Id, "Administrators"))
                {
                    SharePointManager.DeleteFileFromSP(file);
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
            AppUser user = CurrentUser;
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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
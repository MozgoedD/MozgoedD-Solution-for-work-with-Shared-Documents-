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
using Core.Models;
using DAL.Infrastructure;
using BLL.Abstract;

namespace ClientWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        IDocsPageService _service;

        public HomeController(IDocsPageService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            var user = _service.GetCurrentUser();
            if (user != null)
            {
                ViewBag.UserName = user.UserName;
            }
            return View(_service.GetAllFiles());
        }

        public ActionResult Create()
        {
            var user = _service.GetCurrentUser();
            ViewBag.AuthorId = user.Id;
            return View();
        }

        [HttpPost]
        public ActionResult Create(FileCreateModel fileModel)
        {
            if (ModelState.IsValid)
            {
                var file = new AppFileModel
                {
                    Name = Path.GetFileName(fileModel.File.FileName),
                    AuthorId = fileModel.AuthorId,
                };
                var result = _service.UploadFile(file, fileModel.File);
                switch (result)
                {
                    case 0: return RedirectToAction("Index");
                    case 1: return RedirectToAction("LoginNotAccess", "Account");
                    default: return View("Error", new string[] { "File Not Found" });
                }
            }
            return View();
        }

        public FileResult Download(int fileId)
        {
            var file = _service.GetFileById(fileId);
            return File(file.File, "multipart/form-data", file.Name);
        }

        [HttpPost]
        public ActionResult Delete(int fileId)
        {
            var result = _service.DeleteFile(fileId);
            switch (result)
            {
                case 0: return RedirectToAction("Index");
                case 1: return RedirectToAction("LoginNotAccess", "Account");
                default: return View("Error", new string[] { "File Not Found" });
            }
        }

        public ActionResult About()
        {
            var result = _service.UserMenu();
            var user = _service.GetCurrentUser();
            switch (result)
            {
                case 0: return View(user);
                case 1: return RedirectToAction("Index", "Admin");
                default: return RedirectToAction("Index");
            }
        }
    }
}
using BLL.Abstract;
using ClientWebApp.Infrastructure;
using ClientWebApp.Models;
using DAL.Abstract;
using DAL.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace ClientWebApp.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class AdminController : Controller
    {
        IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public ActionResult Index()
        {
            return  View(_adminService.GetAll());
        }

        public async Task<ActionResult> ApprovePage(string id)
        {
            var user = await _adminService.GetById(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ApprovePage(string id, bool isApproved)
        {
            if (isApproved)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var user = await _adminService.GetById(id);

                if (user == null) return View("Error", "User not found");

                var result = await _adminService.ApproveUser(id);

                if (result != null) ModelState.AddModelError("", result);

                return View(user);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Reject(string id)
        {
            var result = await _adminService.RejectUser(id);

            if (result != null) return View("Error", result);

            else return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            var result = await _adminService.DeleteUser(id);

            if (result != null) return View("Error", result);

            else return RedirectToAction("Index");        
        }
    }
}
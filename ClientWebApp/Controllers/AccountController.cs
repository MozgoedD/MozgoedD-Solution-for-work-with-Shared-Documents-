using BLL.Abstract;
using ClientWebApp.Models;
using Core.Models;
using DAL.Abstract;
using DAL.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace ClientWebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        ILoginService _loginService;
        public AccountController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    SecondName = model.SecondName,
                    Patronymic = model.Patronymic,
                    Gender = model.Gender,
                    DOB = model.DOB,
                    Workplace = model.Workplace,
                    JobPosition = model.JobPosition,
                    City = model.City,
                    Country = model.Country,
                    IsApproved = false,
                    RawPassword = System.Web.Security.Membership.GeneratePassword(5, 0)
                };
                var result = await _loginService.CreateUser(user);

                if (result == null) return RedirectToAction("Index");

                else
                {
                    ModelState.AddModelError("", result);
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginNotAccess");
            }

            ViewBag.returnUrl = returnUrl;
            return View();
        }

        public ActionResult LoginNotAccess()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel details, string returnUrl)
        {
            var user = await _loginService.GetUserByCreds(details.Name, details.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Username or Password");
            }
            else
            {
                if (user.IsApproved)
                {
                    await _loginService.Login(user, DefaultAuthenticationTypes.ApplicationCookie);

                    if (returnUrl == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(details);
        }

        [Authorize]
        public ActionResult Logout()
        {
            _loginService.Logout();
            return RedirectToAction("Index", "Home");
        }
    }
}
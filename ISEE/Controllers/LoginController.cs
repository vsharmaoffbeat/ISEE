using ISEEDataModel.Repository;
using ISEEDataModel.Repository.Services;
using ISEEREGION.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ISEEREGION.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var userFactory = new UserFactory();
                var user = userFactory.GetUserById(model.UserName, model.Password);
                //var user = await Factory.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // Keep returnUrl not to kick user out to landing page
            ViewBag.ReturnUrl = returnUrl;

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Admin", "Admin");
            }
        }

        public ActionResult SetSelectedCountry(string lang)
        {
            ISEE.Common.SessionManegment.SessionManagement.Language = lang;
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(ISEE.Common.SessionManegment.SessionManagement.Language);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(ISEE.Common.SessionManegment.SessionManagement.Language);

            //ViewBag.language = lang;

            //return View("Login");
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public JsonResult GetCountries()
        {

            using (ISEEEntities context = new ISEEEntities())
            {
                var ret = context.LanguageLists.Select(d => d.LanguageNickname).Distinct().ToList();
                var selectedImageUrl = string.Format("/images/img/{0}.png", ISEE.Common.SessionManegment.SessionManagement.Language.Trim().ToLower());
                var result = ret.Select(c => new { LanguageNickname = c.Trim(), ImageUrl = string.Format("/images/img/{0}.png", c.Trim().ToLower()) }).ToList();
                return Json(new { CountryList = result, SelectedCountry = ISEE.Common.SessionManegment.SessionManagement.Language, SelectedCountryImageUrl = selectedImageUrl }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
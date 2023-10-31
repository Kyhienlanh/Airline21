using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airline21.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        // GET: Admin/Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult logout()
        {
            Session["TaiKhoan"] = null;
            return RedirectToAction("login", "AccountAdmin", new { area = "" });

        }
    }
}
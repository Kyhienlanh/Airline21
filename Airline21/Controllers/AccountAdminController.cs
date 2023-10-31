using Airline21.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airline21.Controllers
{
    public class AccountAdminController : Controller
    {
        // GET: AccountAdmin
        Entities1 db=new Entities1();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult login()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult login(FormCollection collection)
        {
            var sTenDN = collection["uname"];
            var sMatKhau = collection["psw"];
            if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["Err1"] = "Bạn chưa nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["Err2"] = "Phải nhập mật khẩu";
            }
            else
            {
                // Kiểm tra đăng nhập của khách hàng
              
                User ADMIN = db.Users.SingleOrDefault(n => n.userName == sTenDN && n.passWord == sMatKhau);

                if (ADMIN != null)
                {
                    ViewBag.ThongBao = "Chúc mừng bạn đã đăng nhập thành công";
                    Session["TaiKhoan"] = ADMIN;

                    return Redirect(Url.Action("Index", "HomeAdmin", new { area = "Admin" }));

                }
           
            }
            ViewBag.message = "Wrong";
            return PartialView();
        }



         
        
    }
}
using Airline21.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
namespace Airline21.Areas.Admin.Controllers
{
    public class user1Controller : Controller
    {
        Entities1 db = new Entities1();
        // GET: Admin/Users
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Users(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            var users = db.Users.OrderBy(n => n.IdUser).ToPagedList(iPageNum, iPageSize);
            return View(users);
        }




        [HttpGet]
        public ActionResult Edit(int? id)
        {
            // Tìm người dùng theo ID
            User user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateInput(false)] // You might want to reconsider using ValidateInput(false) for security reasons.
        public ActionResult Edit(User updatedUser)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Find the existing user by ID
                User existingUser = db.Users.Find(updatedUser.IdUser);

                if (existingUser == null)
                {
                    return HttpNotFound(); // Return a 404 status if the user is not found
                }

                // Update the existing user's properties
                existingUser.fullName = updatedUser.fullName;
                existingUser.userName = updatedUser.userName;
                existingUser.passWord = updatedUser.passWord;
                existingUser.email = updatedUser.email;
                existingUser.phone = updatedUser.phone;

                // Save changes to the database
                db.SaveChanges();

                return RedirectToAction("Users"); // Redirect to the "Users" action or an appropriate action.
            }
            else
            {
                // If the model state is not valid, return the view with validation errors
                return View(updatedUser);
            }
        }


        public ActionResult Delete(int id)
        {
            var user = db.Users.SingleOrDefault(n => n.IdUser == id);
            if (user == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(user);
        }



        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id, FormCollection f)
        {
            var user = db.Users.SingleOrDefault(n => n.IdUser == id);
            if (user == null)
            {
                Response.StatusCode = 404;
                return null; // Handle the case when the record is not found
            }

            try
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle any potential exceptions (e.g., database update errors) here.
                // You can log the exception or perform other error-handling tasks.
                // Example: Log.Error(ex, "Error deleting CHUDE record");
                // You can also return an error view or redirect to an error page.
                // Example: return View("Error");
                // Or redirect to a custom error page: return RedirectToAction("Error", "Home");
            }

            return RedirectToAction("Users");
        }
        public ActionResult Details(int id)
        {
            if (id <= 0)
            {
                Response.StatusCode = 400; // Bad Request
                return View("Error"); // Điều hướng đến trang lỗi hoặc trả về một ActionResult khác tùy theo yêu cầu của bạn.
            }

            var user = db.Users.SingleOrDefault(n => n.IdUser == id);
            if (user == null)
            {
                Response.StatusCode = 404; // Not Found
                return View("NotFound"); // Điều hướng đến trang NotFound hoặc trả về một ActionResult khác tùy theo yêu cầu của bạn.
            }

            return View(user);
        }
    }
}
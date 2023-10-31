using Airline21.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airline21.Areas.Admin.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Admin/Default
        Entities1 db = new Entities1();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Ticket(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.Tickets.ToList().OrderBy(n => n.ticketID).ToPagedList(iPageNum, iPageSize));
        }


        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Ticket ticket, FormCollection f, HttpPostedFileBase fFileUpLoad)
        {
            if (fFileUpLoad == null)
            {
                // Xử lý khi không có tệp được tải lên
                // Ví dụ: Thiết lập các giá trị cho ViewBag và trả về View
                ViewBag.price = f["pPrice"];
                ViewBag.update_at = Convert.ToDateTime(f["dupdate_at"]);
                ViewBag.create_at = Convert.ToDateTime(f["dcreate_at"]);
                ViewBag.status = false;
                ViewBag.luggage = f["sLuggage"];
                ViewBag.handluggage = f["sHandLuggage"];


                // Thực hiện lưu thông tin 'flight' vào cơ sở dữ liệu
                if (ModelState.IsValid)
                {
                    db.Tickets.Add(ticket);
                    db.SaveChanges();
                    return RedirectToAction("Ticket");
                }

                // ModelState không hợp lệ, trả về View với 'flight' để hiển thị lỗi
                return View(ticket);
            }
            else
            {
                // Xử lý khi có tệp được tải lên
                if (ModelState.IsValid)
                {

                    db.Tickets.Add(ticket);
                    db.SaveChanges();

                    return RedirectToAction("Ticket");
                }

                // ModelState không hợp lệ, trả về View với 'flight' để hiển thị lỗi
                return View(ticket);
            }
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var ticket = db.Tickets.SingleOrDefault(n => n.ticketID == id);
            if (ticket == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(ticket);
        }



        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id, FormCollection f)
        {
            var ticket = db.Tickets.SingleOrDefault(n => n.ticketID == id);
            if (ticket == null)
            {
                Response.StatusCode = 404;
                return null; // Handle the case when the record is not found
            }

            try
            {
                db.Tickets.Remove(ticket);
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

            return RedirectToAction("Ticket");
        }

        public ActionResult Edit(int id)
        {
            var ticket = db.Tickets.SingleOrDefault(n => n.ticketID == id);
            if (ticket == null)
            {
                Response.StatusCode = 404;
                return null;
            }


            return View(ticket);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Ticket updatedTicket)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Find the existing customer by ID
                var ticket = db.Tickets.SingleOrDefault(n => n.ticketID == id);

                if (ticket == null)
                {
                    return HttpNotFound();
                }

                // Update the existing customer's properties

                ticket.IdFlight = updatedTicket.IdFlight;
                ticket.IdType = updatedTicket.IdType;
                ticket.price = updatedTicket.price;
                ticket.priceSale = updatedTicket.priceSale;
                ticket.created_at = updatedTicket.created_at;
                ticket.updated_at = updatedTicket.updated_at;
                ticket.status = updatedTicket.status;
                ticket.luggage = updatedTicket.luggage;
                ticket.HandLuggage = updatedTicket.HandLuggage;
                ticket.NameTicket = updatedTicket.NameTicket;



                // Save changes to the database
                db.SaveChanges();

                return RedirectToAction("Ticket");
            }
            else
            {
                // If the model state is not valid, return the view with validation errors
                return View(updatedTicket);
            }
        }
        public ActionResult Details(int id)
        {
            var ticket = db.Tickets.SingleOrDefault(n => n.ticketID == id);
            if (ticket == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(ticket);
        }
    }
}
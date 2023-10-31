using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using Airline21.Models;

namespace Airline21.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        Entities1 db = new Entities1();
        // GET: Admin/HomeAdmin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Flight(int? page)
        {
            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(db.Flights.ToList().OrderBy(n => n.IdFlight).ToPagedList(iPageNum, iPageSize));
        }


        public ActionResult Create()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Flight flight, FormCollection f, HttpPostedFileBase fFileUpLoad)
        {
            if (fFileUpLoad == null)
            {
                // Xử lý khi không có tệp được tải lên
                // Ví dụ: Thiết lập các giá trị cho ViewBag và trả về View
                ViewBag.NameFlight = f["sNameFlight"];
                ViewBag.departure_time = Convert.ToDateTime(f["dDeparturetime"]);
                ViewBag.arrival_time = Convert.ToDateTime(f["dArrivaltime"]);
                ViewBag.fromAirport = f["sfromAirport"];
                ViewBag.toAirport = f["stoAirport"];
                ViewBag.FlightDate = Convert.ToDateTime(f["dFlightDate"]);
                

                // Thực hiện lưu thông tin 'flight' vào cơ sở dữ liệu
                if (ModelState.IsValid)
                {
                    db.Flights.Add(flight);
                    db.SaveChanges();
                    return RedirectToAction("Flight");
                }

                // ModelState không hợp lệ, trả về View với 'flight' để hiển thị lỗi
                return View(flight);
            }
            else
            {
                // Xử lý khi có tệp được tải lên
                if (ModelState.IsValid)
                {
                    
                    db.Flights.Add(flight);
                    db.SaveChanges();

                    return RedirectToAction("Flight");
                }

                // ModelState không hợp lệ, trả về View với 'flight' để hiển thị lỗi
                return View(flight);
            }
        }
        public ActionResult Edit(int id)
        {
            var flight = db.Flights.SingleOrDefault(n => n.IdFlight == id);
            if (flight == null)
            {
                Response.StatusCode = 404;
                return null;
            }


            return View(flight);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Flight updatedFlight)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Find the existing customer by ID
                var flight = db.Flights.SingleOrDefault(n => n.IdFlight == id);

                if (flight == null)
                {
                    return HttpNotFound();
                }

                // Update the existing customer's properties

                flight.NameFlight = updatedFlight.NameFlight;
                flight.departure_time = updatedFlight.departure_time;
                flight.arrival_time = updatedFlight.arrival_time;
                flight.fromAirport= updatedFlight.fromAirport;
                flight.toAirport = updatedFlight.toAirport;
                flight.FlightDate = updatedFlight.FlightDate;
                flight.IDBrand = updatedFlight.IDBrand;


                // Save changes to the database
                db.SaveChanges();

                return RedirectToAction("Flight");
            }
            else
            {
                // If the model state is not valid, return the view with validation errors
                return View(updatedFlight);
            }
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var flight = db.Flights.SingleOrDefault(n => n.IdFlight == id);
            if (flight == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(flight);
        }



        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id, FormCollection f)
        {
            var flight = db.Flights.SingleOrDefault(n => n.IdFlight == id);
            if (flight == null)
            {
                Response.StatusCode = 404;
                return null; // Handle the case when the record is not found
            }

            try
            {
                db.Flights.Remove(flight);
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

            return RedirectToAction("Flight");
        }
        public ActionResult Details(int id)
        {
            var flight = db.Flights.SingleOrDefault(n => n.IdFlight == id);
            if (flight == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(flight);
        }

    }
}
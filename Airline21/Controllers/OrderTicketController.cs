using Airline21.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airline21.Controllers
{
    public class OrderTicketController : Controller
    {
        // GET: OrderTicket
        private Entities1 db = new Entities1();
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult index(string Address1, string Address2, string quantity, DateTime date1 = default, DateTime date2 = default)
        {
            if (string.IsNullOrEmpty(Address1) || string.IsNullOrEmpty(Address2) || date1 == default || date2 == default || string.IsNullOrEmpty(quantity))
            {
                ViewBag.Error = "Please fill in all information";
                return View();
            }
            // Handle the valid case here
            return RedirectToAction("FindFlight", new { Address1 , Address2, quantity , date1 , date2 });

        }
        public ActionResult FindFlight(string Address1, string Address2, string quantity, DateTime date1 , DateTime date2)
        {
            var data = from s in db.Flights
                       where s.fromAirport.Equals(Address1) && s.toAirport.Equals(Address2) && (s.FlightDate >= date1 && s.FlightDate <= date2) 
                       select s;

            return View(data);
        }
        public ActionResult TEST()
        {
            return View();
        }

    }
}
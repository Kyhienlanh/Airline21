using Airline21.Models;
using ProjectAirLine39.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectAirLine39.Controllers
{
    public class HomePageController : Controller
    {
        private Entities db =new Entities();
        // GET: HomePage
        public ActionResult Index()
        {
            return PartialView();
        }
        [HttpGet]
        public ActionResult FindFlights()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult FindFlights(String fromAirport, String toAirport, DateTime Tungay, DateTime Denngay)
        {

            return RedirectToAction("SearchResult", new { fromAirport, toAirport, Tungay , Denngay });
        }
        public ActionResult SearchResult(String fromAirport, String toAirport, DateTime Tungay, DateTime Denngay)
        {
            var data = from s in db.Flights
                       where s.fromAirport.Equals(fromAirport) && s.toAirport.Equals(toAirport) && (s.FlightDate >= Tungay && s.FlightDate <= Denngay)
                       select s;
            return View(data);
        }
      
        public ActionResult Ticket(int id)
        {
            var data = from s in db.Tickets
                       join f in db.TypeTickets on s.IdType equals f.IdType
                       where s.IdFlight == id && f.IdType == 1
                       select new TicketTypeViewModel { Ticket = s, TypeTicket = f };

            return View(data.ToList());
        }
        public ActionResult TicketVIP(int id)
        {
            var data = from s in db.Tickets
                       join f in db.TypeTickets on s.IdType equals f.IdType
                       where s.IdFlight == id && f.IdType == 2
                       select new TicketTypeViewModel { Ticket = s, TypeTicket = f };

            return View(data.ToList());
        }
      
        public ActionResult Giave(int id)
        {
            var data = from s in db.Tickets  
                       join f in db.Flights on s.IdFlight equals f.IdFlight
                       where s.ticketID == id
                       select new TicketFlight { Ticket = s, Flight = f };
            ViewBag.TicketID = id;
            return PartialView(data.Single());

        }
        [HttpGet]
        public ActionResult FormOrder(int id)
        {
            ViewBag.ticketid = id;
          
            return PartialView();

        }
        [HttpPost]
        public ActionResult FormOrder(FormCollection form)
        {
            string fullname = form["fullname"];
            string birthdayValue = form["birthday"];
            DateTime birthday;
            DateTime.TryParse(birthdayValue, out birthday);
            string Phone = form["Phone"];
            string Email = form["Email"];
            string CitizenIdentificationCard = form["CitizenIdentificationCard"];
            int ticketID = Convert.ToInt32(form["ticketID"]); // Get the ticketID value from the form

            UserCustomer_Ticket user = new UserCustomer_Ticket
            {
                Name = fullname,
                birthday = birthday,
                PhoneCustomer = Phone,
                EmailCustomer = Email,
                CitizenIdentificationCard = CitizenIdentificationCard,
                ticketID = ticketID
            };

            db.UserCustomer_Ticket.Add(user);
            db.SaveChanges();

            // Redirect to a thank you or success page after saving the data
            return RedirectToAction("Pay", new { user.IDuser_Ticket });
        }

        [HttpGet]
        public ActionResult Pay()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Pay(UserCustomer_Ticket user)
        {
            return View();
        }


    }
}
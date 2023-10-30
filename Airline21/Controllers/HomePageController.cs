using Airline21.Models;
using PayPal.Api;
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
        private Entities1 db =new Entities1();
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
            Ticket dataticket = db.Tickets.FirstOrDefault(s => s.ticketID == id);

            Session["Ticket"] = dataticket;
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
            Session["User"] = user;
          

            db.UserCustomer_Ticket.Add(user);
            db.SaveChanges();

            // Redirect to a thank you or success page after saving the data
            return RedirectToAction("Pay");
        }
        public ActionResult test()
        {
            var ticket = Session["Ticket"] as Ticket;
            return View("Test", ticket);
        }

        public ActionResult Pay()
        {
            UserCustomer_Ticket user = Session["User"] as UserCustomer_Ticket;
           
            if (user == null)
            {
                return RedirectToAction("index");
            }
            var data = from s in db.UserCustomer_Ticket
                       join f in db.Tickets on s.ticketID equals f.ticketID
                       where s.IDuser_Ticket == user.IDuser_Ticket
                       select new Custommer_Ticket { Ticket = f, UserCustomer_Ticket = s };

            return View(data);
        }

        public ActionResult FailureView()
        {
            return View();
        }
        public ActionResult SuccessView()
        {


            return View();
        }

        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Homepage/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                 
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            var ticket = Session["Ticket"] as Ticket;
            UserCustomer_Ticket user = Session["User"] as UserCustomer_Ticket;
            var existingTicket = db.Tickets.Find(ticket.ticketID);
            existingTicket.status = true;
            var userdata = db.UserCustomer_Ticket.Find(user.IDuser_Ticket);
            userdata.status = true;
            db.SaveChanges();

            //on successful payment, show success page to user.  
            return View("SuccessView");
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            // list is a single Ticket object, not a collection
            var ticket = Session["Ticket"] as Ticket;
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = ticket.NameTicket.ToString(),
                currency = "USD",
                price = ticket.price.ToString(),
                quantity = "1",
                sku = "sku"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = ticket.price.ToString()
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = ticket.price.ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }








    }
}
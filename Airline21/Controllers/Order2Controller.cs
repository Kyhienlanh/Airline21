using Airline21.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airline21.Controllers
{
    public class Order2Controller : Controller
    {
        // GET: Order2
        private Entities1 db = new Entities1();
        [HttpGet]
        public ActionResult Index()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult index(string Address1, string Address2, string quantity, DateTime date1 = default, DateTime date2 = default)
        {
            if (string.IsNullOrEmpty(Address1) || string.IsNullOrEmpty(Address2) || date1 == default || date2 == default || string.IsNullOrEmpty(quantity))
            {
                ViewBag.Error = "Please fill in all information";

                return PartialView();
            }
            // Handle the valid case here
            return RedirectToAction("FindFlight", new { Address1, Address2, quantity, date1, date2 });

        }
        public ActionResult FindFlight(string Address1, string Address2, string quantity, DateTime date1, DateTime date2)
        {
            int quantityValue;
            if (!int.TryParse(quantity, out quantityValue))
            {
                return View("loi");
            }

            Session["quantityValue"] = quantityValue;
            ViewBag.count = quantityValue;

            var data = from s in db.Flights
                       where s.fromAirport.Equals(Address1)
                           && s.toAirport.Equals(Address2)
                           && DbFunctions.TruncateTime(s.FlightDate) == date1.Date
                           && s.quantity >= quantityValue
                       select s;

            var data2 = (from s in db.Flights
                         where s.fromAirport.Equals(Address2)
                             && s.toAirport.Equals(Address1)
                             && DbFunctions.TruncateTime(s.FlightDate) == date2.Date
                             && s.quantity >= quantityValue
                         select s).ToList();

            Session["data2"] = data2;
            if (!data.Any() || !data2.Any())
            {
                return RedirectToAction("test");
            }

            return View(data);
        }

        public ActionResult FindFlight2()
        {
            ViewBag.count = Session["quantityValue"];
            List<Flight> data2 = Session["data2"] as List<Flight>;
            return View(data2);
        }
        [HttpGet]
        public ActionResult FlightNomal1(string id, string count)
        {

            int.TryParse(count, out int value);
            ViewBag.FormCount = value;
            if (int.TryParse(id, out int flightId))
            {
                var data = from s in db.Flights where s.IdFlight == flightId select s;
                var flight = data.SingleOrDefault();
                if (int.TryParse(flight.generalprice, out int generalPrice))
                {
                    var total = generalPrice * value;


                    ViewBag.Total = total;
                    Session["Total1"] = total;
                    Session["Flight1"] = flight;
                    Session["IDFlight1"] = flight.IdFlight;
                    return RedirectToAction("FindFlight2");

                }
                else
                {
                    return RedirectToAction("Error");
                }
            }
            else
            {

                return RedirectToAction("Error");
            }
        }
        [HttpGet]
        public ActionResult FlightNomal2(string id, string count)
        {

            int.TryParse(count, out int value);
            ViewBag.FormCount = value;
            if (int.TryParse(id, out int flightId))
            {
                var data = from s in db.Flights where s.IdFlight == flightId select s;
                var flight = data.SingleOrDefault();
                if (int.TryParse(flight.generalprice, out int generalPrice))
                {
                    var total = generalPrice * value;


                    ViewBag.Total = total;
                    Session["Total2"] = total;
                    Session["Flight2"] = flight;
                    Session["IDFlight2"] = flight.IdFlight;
                    return RedirectToAction("FormInfor", new { id = id, count = count });

                }
                else
                {
                    return RedirectToAction("Error");
                }
            }
            else
            {

                return RedirectToAction("Error");
            }
        }

        public ActionResult test()
        {
            int total1 = (int)Session["Total1"];
            int total2 = (int)Session["Total2"];
            int total = total1 + total2;
            Session["totalFlight"]=total;
            return PartialView(total);
        }
        [HttpGet]
        public ActionResult FormInfor(string id, string count)
        {
            /*var AccountData = Session["Account"];
            if (AccountData == null)
            {
                // Lưu URL hiện tại vào biến returnUrl
                string returnUrl = Request.Url.AbsoluteUri;

                // Chuyển hướng đến trang đăng nhập và truyền returnUrl như một tham số
                return RedirectToAction("Login", "Account", new { returnUrl });
            }*/
            int.TryParse(count, out int value);
            ViewBag.FormCount = value;
            if (int.TryParse(id, out int flightId))
            {
                var data = from s in db.Flights where s.IdFlight == flightId select s;
                var flight = data.SingleOrDefault();
                if (int.TryParse(flight.generalprice, out int generalPrice))
                {
                    var total = generalPrice * value;


                    ViewBag.Total = total;
                    Session["Total"] = total;
                    Session["Flight"] = flight;
                    Session["IDFlight"] = flight.IdFlight;
                    return View(flight);
                }
                else
                {
                    return RedirectToAction("Error");
                }
            }
            else
            {

                return RedirectToAction("Error");
            }
        }
        [HttpPost]
        public ActionResult FormInfor(FormCollection form)
        {
            string formCount = form["FormCount"];
            int.TryParse(formCount, out int value);

            List<UserCustomer_Ticket> users = new List<UserCustomer_Ticket>();
            for (int i = 1; i <= value; i++)
            {
               for(int j = 0; j < 2; j++)
                {
                    string fullname = form["fullname " + i];
                    string birthdayValue = form["birthday " + i];
                    DateTime birthday;
                    DateTime.TryParse(birthdayValue, out birthday);
                    string Phone = form["Phone " + i];
                    string Email = form["Email " + i];
                    string CitizenIdentificationCard = form["CitizenIdentificationCard " + i];

                    UserCustomer_Ticket user = new UserCustomer_Ticket
                    {
                        Name = fullname,
                        birthday = birthday,
                        PhoneCustomer = Phone,
                        EmailCustomer = Email,
                        CitizenIdentificationCard = CitizenIdentificationCard
                    };

                    db.UserCustomer_Ticket.Add(user);
                    db.SaveChanges();
                    users.Add(user);
                }
            }

            // Store the user information in the session
            Session["Users"] = users;

            return RedirectToAction("Service");
        }
        public ActionResult DetailFlight1()
        {

            int total1 = (int)Session["Total1"];

            ViewData["total1"] = total1;
            int quantity = (int)Session["quantityValue"];
            ViewData["quantity"] = quantity;
            var data = Session["Flight1"] as Flight;
            return PartialView(data);
        }
        public ActionResult DetailFlight2()
        {

            int total2 = (int)Session["Total2"];
            int quantity = (int)Session["quantityValue"];
            ViewData["total2"] = total2;
            ViewData["quantity"] = quantity;
            var data = Session["Flight2"] as Flight;
            return PartialView(data);
        }
        [HttpGet]
        public ActionResult Service()
        {
            List<UserCustomer_Ticket> storedUsers = Session["Users"] as List<UserCustomer_Ticket>;

            return View(storedUsers);
        }
        [HttpPost]
        public ActionResult Service(FormCollection form)
        {
            List<UserCustomer_Ticket> storedUsers = Session["Users"] as List<UserCustomer_Ticket>;
            List<service> servicePeople = new List<service>();

            if (storedUsers != null)
            {
                try
                {
                    int totalService = 0;

                    for (int i = 0; i < storedUsers.Count; i++)
                    {
                        string formCount = form["txtSoLuong " + i];
                        int.TryParse(formCount, out int value);

                        string id = form["id " + i];
                        int.TryParse(id, out int iduser);
                        UserCustomer_Ticket user = db.UserCustomer_Ticket.SingleOrDefault(n => n.IDuser_Ticket == iduser);
                        string name = form["name " + i];
                        string securityService = form["securityService " + i];
                        int TotalHuman = 0;
                        if (user != null)
                        {
                            user.extraluggage = value;
                            totalService += value * 5;
                            TotalHuman += value * 5;


                            user.securityService = securityService;

                            if (securityService == "Yes")
                            {
                                totalService += 20;
                                TotalHuman += 20;
                            }
                        }

                        service user1 = new service(iduser, name, value, securityService, TotalHuman);
                        servicePeople.Add(user1);
                    }

                    db.SaveChanges();


                    Session["servicePeople"] = servicePeople;
                    Session["totalService"] = totalService;

                    return RedirectToAction("ChoseFlight1");
                    //return RedirectToAction("servicePeople");
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                    return RedirectToAction("Error");
                }
            }

            return RedirectToAction("Error");
        }
        [HttpGet]
        public ActionResult ChoseFlight1()
        {
            List<UserCustomer_Ticket> list2 = Session["Users"] as List<UserCustomer_Ticket>;

            ViewData["List2"] = list2;
            var data = Session["IDFlight1"];
            if (data != null)
            {
                string idFlight = data.ToString();
                int.TryParse(idFlight, out int value);

                var data1 = from s in db.Tickets
                            where s.IdFlight == value
                            select s;
                return PartialView(data1.ToList());
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        [HttpPost]
        public ActionResult ChoseFlight1(FormCollection form)
        {
            List<UserCustomer_Ticket> storedUsers = Session["Users"] as List<UserCustomer_Ticket>;
            List<int> IdTicket = new List<int>();
            for (int i = 0; i < storedUsers.Count; i=i+2)
            {
                string id = form["id " + i];
                int.TryParse(id, out int iduser);
                UserCustomer_Ticket user = db.UserCustomer_Ticket.SingleOrDefault(n => n.IDuser_Ticket == iduser);
                string ticketId = form["place " + i];
                int.TryParse(ticketId, out int ticket);
                user.ticketID = ticket;
                IdTicket.Add(ticket);
            }
            Session["IdTicket"] = IdTicket;
            db.SaveChanges();
            return RedirectToAction("ChoseFlight2");

        }
        [HttpGet]
        public ActionResult ChoseFlight2()
        {
            List<UserCustomer_Ticket> list2 = Session["Users"] as List<UserCustomer_Ticket>;

            ViewData["List2"] = list2;
            var data = Session["IDFlight2"];
            if (data != null)
            {
                string idFlight = data.ToString();
                int.TryParse(idFlight, out int value);

                var data1 = from s in db.Tickets
                            where s.IdFlight == value
                            select s;
                return PartialView(data1.ToList());
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        [HttpPost]
        public ActionResult ChoseFlight2(FormCollection form)
        {
            List<UserCustomer_Ticket> storedUsers = Session["Users"] as List<UserCustomer_Ticket>;
            List<int> IdTicket = new List<int>();
            for (int i = 1; i < storedUsers.Count; i = i + 2)
            {
                string id = form["id " + i];
                int.TryParse(id, out int iduser);
                UserCustomer_Ticket user = db.UserCustomer_Ticket.SingleOrDefault(n => n.IDuser_Ticket == iduser);
                string ticketId = form["place " + i];
                int.TryParse(ticketId, out int ticket);
                user.ticketID = ticket;
                IdTicket.Add(ticket);
            }
            Session["IdTicket1"] = IdTicket;
            db.SaveChanges();
            return RedirectToAction("pay");

        }
        public ActionResult TotalAmount()
        {
            object totalServiceObject = Session["totalService"];
            int totalService = (int)totalServiceObject;
            int total1 = (int)Session["Total1"];
            int total2 = (int)Session["Total2"];
            int TotalAmount1 = totalService + total1 + total2;
           
            return PartialView(TotalAmount1);
        }
        public ActionResult pay()
        {
            List<UserCustomer_Ticket> storedUsers = Session["Users"] as List<UserCustomer_Ticket>;
            List<service> list2 = Session["servicePeople"] as List<service>;
            ViewData["List2"] = list2;
            int total1 = (int)Session["Total1"];
            ViewBag.total1 = total1;
            int total2 = (int)Session["Total2"];
            ViewBag.total2 = total2;
            object totalServiceObject = Session["totalService"];
            int totalService = (int)totalServiceObject;
            int TotalAmount = totalService+total2+total1;
            Session["TotalAmount1"] = TotalAmount;
            ViewBag.TotalAmount = TotalAmount;

            return View(storedUsers);
        }
        public ActionResult hi()
        {
            int a = (int)Session["IDFlight2"];
            return View(a);
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
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Order2/PaymentWithPayPal?";
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
            var Account = Session["Account"];
            if (Account != null)
            {
                var userlogin = db.AspNetUsers.SingleOrDefault(u => u.Email == Account);

                List<UserCustomer_Ticket> storedUsers = Session["Users"] as List<UserCustomer_Ticket>;
                foreach (var item in storedUsers)
                {
                    UserCustomer_Ticket user = db.UserCustomer_Ticket.SingleOrDefault(n => n.IDuser_Ticket == item.IDuser_Ticket);
                    user.status = true;
                    user.IDAccount = userlogin.Id.ToString();
                }
            }
            else
            {
                List<UserCustomer_Ticket> storedUsers = Session["Users"] as List<UserCustomer_Ticket>;
                foreach (var item in storedUsers)
                {
                    UserCustomer_Ticket user = db.UserCustomer_Ticket.SingleOrDefault(n => n.IDuser_Ticket == item.IDuser_Ticket);
                    user.status = true;

                }
            }

            List<int> IdTicket = Session["IdTicket"] as List<int>;
            foreach (var item in IdTicket)
            {
                Ticket ticket = db.Tickets.SingleOrDefault(n => n.ticketID == (int)item);
                ticket.status = true;
            }
            List<int> IdTicket1 = Session["IdTicket1"] as List<int>;
            foreach (var item in IdTicket1)
            {
                Ticket ticket = db.Tickets.SingleOrDefault(n => n.ticketID == (int)item);
                ticket.status = true;
            }

            db.SaveChanges();

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

            //create itemlist and add item objects to it  
            int TotalAmount = 0; // Default value or some meaningful default for your application
            if (Session["TotalAmount1"] != null && int.TryParse(Session["TotalAmount1"].ToString(), out int amount1))
            {
                TotalAmount = amount1;
            }

            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Ticket",
                currency = "USD",
                price = TotalAmount.ToString(),
                quantity = "1",
                sku = "Ticket Flight"
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
                subtotal = TotalAmount.ToString()
            };
            var total = (Convert.ToDouble(details.tax) + Convert.ToDouble(details.shipping) + Convert.ToDouble(details.subtotal)).ToString();
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = total.ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
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
using Airline21.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Airline21.Controllers
{
    public class Order1Controller : Controller
    {
        private Entities1 db = new Entities1();
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult index(string Address1, string Address2, string quantity, DateTime date1 = default)
        {
            if (string.IsNullOrEmpty(Address1) || string.IsNullOrEmpty(Address2) || date1 == default || string.IsNullOrEmpty(quantity))
            {
                ViewBag.Error = "Please fill in all information";

                return View();
            }
            // Handle the valid case here
            return RedirectToAction("FindFlight", new { Address1, Address2, quantity, date1});

        }
        public ActionResult FindFlight(string Address1, string Address2, string quantity, DateTime date1)
        {
            int quantityValue;
            if (!int.TryParse(quantity, out quantityValue))
            {
                return View("loi");
            }
            Session["quantityValue"] = quantityValue;
            var data = from s in db.Flights
                       where s.fromAirport.Equals(Address1) && s.toAirport.Equals(Address2) && (s.FlightDate == date1) && s.quantity >= quantityValue
                       select s;
            ViewBag.count = quantityValue;

            return View(data);
        }

        [HttpGet]
        public ActionResult FormInfor(string id, string count)
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
        public ActionResult DetailFlight()
        {

            int total = (int)Session["Total"];

            ViewData["total"] = total;
            var data = Session["Flight"] as Flight ;
            return PartialView(data);
        }
        [HttpGet]
        public ActionResult FormInfor2(string id, string count)
        {

            int.TryParse(count, out int value);
            ViewBag.FormCount = value;
            if (int.TryParse(id, out int flightId))
            {
                var data = from s in db.Flights where s.IdFlight == flightId select s;
                var flight = data.SingleOrDefault();



                if (int.TryParse(flight.merchantprice, out int merchantprice))
                {
                    var total = merchantprice * value;
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
        public ActionResult FormInfor2(FormCollection form)
        {
            string formCount = form["FormCount"];
            int.TryParse(formCount, out int value);

            List<UserCustomer_Ticket> users = new List<UserCustomer_Ticket>();
            for (int i = 1; i <= value; i++)
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

            // Store the user information in the session
            Session["Users"] = users;

            return RedirectToAction("Service");
        }

        [HttpPost]
        public ActionResult FormInfor(FormCollection form)
        {
            string formCount = form["FormCount"];
            int.TryParse(formCount, out int value);

            List<UserCustomer_Ticket> users = new List<UserCustomer_Ticket>();
            for (int i = 1; i <= value; i++)
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

            // Store the user information in the session
            Session["Users"] = users;

            return RedirectToAction("Service");
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


                    Session["servicePeople"] = servicePeople ;
                    Session["totalService"] = totalService;

                     return RedirectToAction("ChoseFlight");
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
        public ActionResult ChoseFlight()
        {
            List<UserCustomer_Ticket> list2 = Session["Users"] as List<UserCustomer_Ticket>;
           
            ViewData["List2"] = list2;
            var data = Session["IDFlight"];
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
        public ActionResult ChoseFlight(FormCollection form)
        {
            List<UserCustomer_Ticket> storedUsers = Session["Users"] as List<UserCustomer_Ticket>;
            for(int i = 0; i < storedUsers.Count; i++)
            {
                string id = form["id " + i];
                int.TryParse(id, out int iduser);
                UserCustomer_Ticket user = db.UserCustomer_Ticket.SingleOrDefault(n => n.IDuser_Ticket == iduser);
                string ticketId = form["place " + i];
                int.TryParse(ticketId, out int ticket);
                user.ticketID = ticket;
            }
            db.SaveChanges();
            return RedirectToAction("pay");
          
        }
       


        public ActionResult servicePeople()
        {
            List<service>servicePeople = Session["servicePeople"] as List<service>;
            return PartialView(servicePeople);
        }


        public ActionResult totalService()
        {
            object totalServiceObject = Session["totalService"];
            if (totalServiceObject != null && totalServiceObject is int)
            {
                int totalService = (int)totalServiceObject;
                return PartialView(totalService);
            }
            else
            {
              
                return RedirectToAction("Error");
            }
        }
        public ActionResult TotalAmount()
        {
            object totalServiceObject = Session["totalService"];
            int totalService = (int)totalServiceObject;
            int total = (int)Session["Total"];
            int TotalAmount1 = totalService + total;
            Session["TotalAmount"] = TotalAmount1;
            return PartialView(TotalAmount1);
        }
        public int Total()
        {
            object totalServiceObject = Session["totalService"];
            int totalService = (int)totalServiceObject;
            int total = (int)Session["Total"];
            int TotalAmount1 = totalService + total;
            return TotalAmount1;
        }
        public ActionResult pay()
        {
            List<UserCustomer_Ticket> storedUsers = Session["Users"] as List<UserCustomer_Ticket>;
            List<service> list2 = Session["servicePeople"] as List<service>;
            ViewData["List2"] = list2;
            int total = (int)Session["Total"];
            ViewBag.total = total;
            int TotalAmount = (int)Session["TotalAmount"];
            ViewBag.TotalAmount = TotalAmount;
          
            return View(storedUsers);
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
            var Account = Session["Account"];
            //var IdAccount = from s in db.AspNetUsers where s.Email.Equals(Account) select s.Id;
            var userlogin = db.AspNetUsers.SingleOrDefault(u => u.Email == Account);
            userdata.IDAccount = userlogin.Id.ToString();
            db.SaveChanges();
            //var IdAccount = from s in db.AspNetUsers where s.Email.Equals(Account) select s.Id;


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


        [HttpGet]
        public ActionResult ChosePlace()
        {
            List<UserCustomer_Ticket> storedUsers = Session["Users"] as List<UserCustomer_Ticket>;
           
            return View(storedUsers);
        }
        [HttpPost]
        public ActionResult ChosePlace(FormCollection f)
        {
          

            return View();
        }








      




        public ActionResult test(int value)
        {
            ViewBag.test = value;
            return View();
        }



        public ActionResult ChooseAPlace()
        {
            return View();
        }
    }
}
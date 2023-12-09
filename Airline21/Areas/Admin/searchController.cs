using Airline21.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airline21.Areas.Admin
{
    public class searchController : Controller
    {
        // GET: Admin/search
        private Entities1 db = new Entities1();
      
        public ActionResult Index(string searchString)
        {
            var result = db.UserCustomer_Ticket
     .Where(p => p.CitizenIdentificationCard.Contains(searchString) && p.status==true)
     .ToList();



            return View(result);
        }
    }
}
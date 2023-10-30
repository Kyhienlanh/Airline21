using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Airline21.Models
{
    public class InvoiceDetailsViewModel
    {
        public int TicketID { get; set; }
        public string TicketName { get; set; }
        public decimal TicketPrice { get; set; }
        public int CustomerTicketID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
    }
}
using Airline21.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectAirLine39.Models
{
    public class TicketFlight
    {
        public Ticket Ticket { get; set; }
        public Flight Flight { get; set; }
    }
}
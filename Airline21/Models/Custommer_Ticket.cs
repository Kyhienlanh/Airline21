using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Airline21.Models
{
    public class Custommer_Ticket
    {
        public Ticket Ticket { get; set; }
        public UserCustomer_Ticket UserCustomer_Ticket { get; set; }
    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Airline21.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class invoice
    {
        public int InvoiceID { get; set; }
        public Nullable<bool> status { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<int> IDuser_Ticket { get; set; }
    
        public virtual UserCustomer_Ticket UserCustomer_Ticket { get; set; }
    }
}
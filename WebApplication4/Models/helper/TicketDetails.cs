using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class TicketDetails
    {
        public Ticket ticketdetails { get; set; }
        public TicketIndex ticketindex { get; set; }
        public SelectList ticketassign { get; set; }
    }
}
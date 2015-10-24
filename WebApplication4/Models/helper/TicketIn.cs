using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class TicketIn
    {
        public Ticket master { get; set; }
        public ListboxTicket lists { get; set; }
    }
}
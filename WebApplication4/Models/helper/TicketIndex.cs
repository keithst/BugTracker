using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class TicketIndex
    {
        public IList<Ticket> tickets { get; set; }
        public IList<UserTicketList> accessin { get; set; }
        public UserAccess helper { get; set; }
    }
}
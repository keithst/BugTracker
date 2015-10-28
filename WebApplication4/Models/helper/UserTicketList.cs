using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class UserTicketList
    {
        public bool ownerconfirmed { get; set; }
        public bool assignconfirmed { get; set; }
        public bool isinproject { get; set; }
        public int ticketin { get; set; }
    }
}
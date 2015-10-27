using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class Userhelp
    {
        public MultiSelectList users { get; set; }
        public string user { get; set; }
        public Projects pid { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class ReportList
    {
        public string name { get; set; }
        public decimal count { get; set; }
        public decimal percentage { get; set; }
        public int total { get; set; }
    }
}
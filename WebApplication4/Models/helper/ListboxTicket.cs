using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class ListboxTicket
    {
        public SelectList StatusInput { get; set; }
        public string Status { get; set; }
        public SelectList PriorityInput { get; set; }
        public string Priority { get; set; }
        public SelectList TypeInput { get; set; }
        public string Type { get; set; }
        public SelectList ProjectInput { get; set; }
        public string Project { get; set; }
    }
}
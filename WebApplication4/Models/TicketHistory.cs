using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string TicketFieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public System.DateTimeOffset Changed { get; set; }
        public string HistoryUserId { get; set; }

        public virtual ApplicationUser HistoryUser { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
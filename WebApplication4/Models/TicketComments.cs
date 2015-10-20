using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class TicketComments
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public string AttachURL { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public int TicketId { get; set; }
        public string CommentUserId { get; set; }

        public virtual ApplicationUser CommentUser { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class Ticket
    {
        public Ticket()
        {
            this.TicketHistory = new HashSet<TicketHistory>();
            this.TicketComments = new HashSet<TicketComments>();
            this.TicketNotify = new HashSet<TicketNotify>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public string OwnerId { get; set; }
        public string AssignedId { get; set; }

        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual Projects Project { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public virtual ApplicationUser Assigned { get; set; }
        public virtual ICollection<TicketHistory> TicketHistory { get; set; }
        public virtual ICollection<TicketComments> TicketComments { get; set; }
        public virtual ICollection<TicketNotify> TicketNotify { get; set; }
    }
}
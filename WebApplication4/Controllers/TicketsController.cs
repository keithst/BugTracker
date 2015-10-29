using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebApplication4.Models.helper;
using PagedList;

namespace WebApplication4.Models
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketIn tickets = new TicketIn();
        private TicketIndex indexinput = new TicketIndex();
        private UserAccess helper = new UserAccess();
        private TicketDetails ticketd = new TicketDetails();
        private IList<ApplicationUser> userassign = new List<ApplicationUser>();

        // GET: Tickets
        [Authorize]
        public ActionResult Index()
        {
            var ticketdb = db.Tickets.Include(t => t.Assigned).Include(t => t.Owner).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            indexinput.tickets = ticketdb.OrderBy(x => x.Id).ToList();

            var idin = User.Identity.GetUserId();
            indexinput.accessin = helper.UserisOwnerorAssigned(idin, indexinput.tickets);
            indexinput.helper = helper;

            return View(indexinput);
            
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        [Authorize]
        public ActionResult Create()
        {
            ListboxTicket lbt = new ListboxTicket();
            lbt.PriorityInput = new SelectList(db.Priorities, "Priority", "Priority");
            lbt.StatusInput = new SelectList(db.Status, "Status", "Status");
            lbt.TypeInput = new SelectList(db.Types, "Type", "Type");
            lbt.ProjectInput = new SelectList(db.Projects, "Project", "Project");
            tickets.lists = lbt;
            return View(tickets);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerId,AssignedId")] Ticket ticket, string Title, string Description, string Status, string Type, string Priority, string Project)
        {
            if (ModelState.IsValid)
            {
                var queryp = db.Priorities.AsQueryable();
                var querys = db.Status.AsQueryable();
                var querypr = db.Projects.AsQueryable();
                var queryty = db.Types.AsQueryable();
                if(!string.IsNullOrWhiteSpace(Priority))
                {
                    var queryt = queryp.Where(x => x.Priority == Priority).Select(y => y.Id).Single();
                    ticket.TicketPriorityId = queryt;
                }
                if(!string.IsNullOrWhiteSpace(Status))
                {
                    var queryt = querys.Where(x => x.Status == Status).Select(y => y.Id).Single();
                    ticket.TicketStatusId = queryt;
                }
                if(!string.IsNullOrWhiteSpace(Project))
                {
                    var queryt = querypr.Where(x => x.Project == Project).Select(y => y.Id).Single();
                    ticket.ProjectId = queryt;
                }
                if(!string.IsNullOrWhiteSpace(Type))
                {
                    var queryt = queryty.Where(x => x.Type == Type).Select(y => y.Id).Single();
                    ticket.TicketTypeId = queryt;
                }
                ticket.Title = Title;
                ticket.Description = Description;
                ticket.Created = System.DateTimeOffset.Now;
                ticket.OwnerId = User.Identity.GetUserId();
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            var query = db.ProjectUsers.Where(x => x.Project.Project == ticket.Project.Project).ToList();
            foreach(var item in query)
            {
                var user = db.Users.Where(x => x.Id == item.ProjectUserId).Single();
                userassign.Add(user);
            }
            ticketd.ticketassign = new SelectList(userassign, "UserName", "UserName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Project", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.Priorities, "Id", "Priority", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.Status, "Id", "Status", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.Types, "Id", "Type", ticket.TicketTypeId);
            ticketd.ticketdetails = ticket;
            return View(ticketd);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerId,AssignedId")] Ticket ticket, string Assigned)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(x => x.UserName == Assigned).Single();
                ticket.AssignedId = user.Id;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Project", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.Priorities, "Id", "Priority", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.Status, "Id", "Status", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.Types, "Id", "Type", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

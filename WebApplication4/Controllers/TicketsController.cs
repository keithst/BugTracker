﻿using System;
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
using System.IO;
using System.Data.Entity.Core.Objects;
using System.Configuration;
using SendGrid;
using System.Net.Mail;

namespace WebApplication4.Models
{
    [RequireHttps]
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketIn tickets = new TicketIn();
        private TicketIndex indexinput = new TicketIndex();
        private UserAccess helper = new UserAccess();
        private TicketDetails ticketd = new TicketDetails();
        private IList<ApplicationUser> userassign = new List<ApplicationUser>();

        // GET: Tickets
        public ActionResult Index(string priority, string project)
        {
            var ticketdb = db.Tickets.Include(t => t.Assigned).Include(t => t.Owner).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            if (!string.IsNullOrWhiteSpace(priority))
            {
                if (!string.IsNullOrWhiteSpace(project))
                {
                    indexinput.tickets = ticketdb.Where(x => (x.TicketPriority.Priority == priority) & (x.Project.Project == project)).OrderBy(y => y.Id).ToList();
                }
                else
                {
                    indexinput.tickets = ticketdb.Where(x => (x.TicketPriority.Priority == priority)).OrderBy(y => y.Id).ToList();
                }
            }
            else
            {
                indexinput.tickets = ticketdb.OrderBy(x => x.Id).ToList();
            }

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
            ticketd.ticketdetails = ticket;
            ticketd.accessin = helper.UserisOwnerorAssignedSingle(User.Identity.GetUserId(), ticket);
            ticketd.historyin = db.Histories.Where(x => x.TicketId == id).ToList();
            return View(ticketd);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "Id,Comment,AttachURL,Created,TicketId,CommentUserId")] TicketComments ticketcomment, int id, HttpPostedFileBase AttachURLin)
        {
            if (AttachURLin != null && AttachURLin.ContentLength > 0)
            {
                var ext = Path.GetExtension(AttachURLin.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg"
                    && ext != ".bmp" && ext != ".txt" && ext != ".pdf" && ext != ".rtf")
                {
                    ModelState.AddModelError("AttachURL", "Invalid Format.");
                }
            }
            if (ModelState.IsValid)
            {
                if (AttachURLin != null)
                {
                    var filePath = "/Uploads/";
                    var absPath = Server.MapPath("~" + filePath);
                    Directory.CreateDirectory(absPath);
                    ticketcomment.AttachURL = filePath + AttachURLin.FileName;
                    AttachURLin.SaveAs(Path.Combine(absPath, AttachURLin.FileName));
                }
            }
            ticketcomment.Created = System.DateTimeOffset.Now;
            ticketcomment.TicketId = id;
            ticketcomment.CommentUserId = User.Identity.GetUserId();
            db.Comments.Add(ticketcomment);
            db.SaveChanges();
            return RedirectToAction("Details", id);
        }

        // GET: Tickets/Create
        [Authorize]
        public ActionResult Create()
        {
            ListboxTicket lbt = new ListboxTicket();
            lbt.PriorityInput = new SelectList(db.Priorities, "Priority", "Priority");
            lbt.StatusInput = new SelectList(db.Status.Where(x => x.Status == "Open"), "Status", "Status", "Open");
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
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerId,AssignedId")] Ticket ticket, string Title, string Status, string Type, string Priority, string Project)
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
                ticket.Created = System.DateTimeOffset.Now;
                ticket.OwnerId = User.Identity.GetUserId();

                TicketNotify note = new TicketNotify();
                note.TicketId = ticket.Id;
                note.NotifyUserId = ticket.OwnerId;
                db.Notifications.Add(note);
                

                UserRoleHelper helper = new UserRoleHelper();
                var manager = db.ProjectUsers.Where(x => x.ProjectId == ticket.ProjectId).ToList();
                foreach(var item in manager)
                {
                    if(helper.IsUserInRole(item.ProjectUserId, "ProjectManager"))
                    {
                        TicketNotify note2 = new TicketNotify();
                        note2.TicketId = ticket.Id;
                        note2.NotifyUserId = item.ProjectUserId;
                        db.Notifications.Add(note2);
                    }
                }


                var emails = db.Notifications.Where(x => x.TicketId == ticket.Id).Select(y => y.NotifyUser.Email).ToList();
                var username = ConfigurationManager.AppSettings["SendGridUserName"];
                var password = ConfigurationManager.AppSettings["SendGridPassword"];
                var from = ConfigurationManager.AppSettings["ContactEmail"];

                foreach (var email in emails)
                {
                    SendGridMessage myMessage = new SendGridMessage();
                    myMessage.AddTo(email);
                    myMessage.From = new MailAddress(from);
                    myMessage.Subject = "Notification for Ticket #" + ticket.Id;
                    myMessage.Html = "Ticket #" + ticket.Id + " has been created";
                    var credentials = new NetworkCredential(username, password);

                    var transportWeb = new Web(credentials);

                    transportWeb.DeliverAsync(myMessage);
                }

                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            UserRoleHelper helper = new UserRoleHelper();
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
            bool found = false;
            foreach(var item in query)
            {
                var user = db.Users.Where(x => x.Id == item.ProjectUserId).Single();
                if (helper.IsUserInRole(user.Id, "Developer"))
                {
                    userassign.Add(user);
                    found = true;
                }
            }
            if (found)
            {
                if(!string.IsNullOrWhiteSpace(ticket.AssignedId))
                {
                    ticketd.ticketassign = new SelectList(userassign, "UserName", "UserName", ticket.Assigned.UserName);
                }
                else
                {
                    ticketd.ticketassign = new SelectList(userassign, "UserName", "UserName");
                }
            }
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
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerId,AssignedId")] Ticket ticket, string Assigned, int ProjectIn)
        {
            bool sendemail = false;
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(Assigned))
                {
                    var user = db.Users.Where(x => x.UserName == Assigned).Single();
                    ticket.AssignedId = user.Id;
                }

                ticket.ProjectId = ProjectIn;
                var dbin = db.Tickets.Single(x => x.Id == ticket.Id);
                db.Entry(dbin).CurrentValues.SetValues(ticket);

                var cnames = db.Entry(dbin).CurrentValues.PropertyNames;
                foreach (var curr in cnames)
                {
                    var oldvalue = "";
                    var newvalue = "";
                    if (db.Entry(dbin).Property(curr).OriginalValue != null)
                    {
                        oldvalue = db.Entry(dbin).Property(curr).OriginalValue.ToString();
                    }
                    if (db.Entry(dbin).Property(curr).CurrentValue != null)
                    {
                        newvalue = db.Entry(dbin).Property(curr).CurrentValue.ToString();
                    }
                    if (oldvalue != newvalue)
                    {
                        TicketHistory hist = new TicketHistory();
                        hist.TicketFieldName = curr;
                        hist.OldValue = oldvalue;
                        hist.NewValue = newvalue;
                        hist.HistoryUserId = User.Identity.GetUserId();
                        hist.TicketId = ticket.Id;
                        hist.Changed = System.DateTimeOffset.Now;
                        db.Histories.Add(hist);
                        if(curr == "AssignedId" || curr == "TicketPriorityId" || curr == "TicketStatusId")
                        {
                            sendemail = true;
                        }
                    }
                }
                if (db.Entry(dbin).Property("AssignedId").CurrentValue != null)
                {
                    if(!db.Entry(dbin).Property("AssignedId").OriginalValue.ToString().Equals(db.Entry(dbin).Property("AssignedId").CurrentValue.ToString()))
                    {
                        TicketNotify note = new TicketNotify();
                        note.TicketId = ticket.Id;
                        note.NotifyUserId = ticket.AssignedId;
                        db.Notifications.Add(note);
                        db.SaveChanges();
                    }
                }
                if(sendemail)
                {
                    var emails = db.Notifications.Where(x => x.TicketId == ticket.Id).ToList();
                    var username = ConfigurationManager.AppSettings["SendGridUserName"];
                    var password = ConfigurationManager.AppSettings["SendGridPassword"];
                    var from = ConfigurationManager.AppSettings["ContactEmail"];

                    foreach (var email in emails)
                    {
                        SendGridMessage myMessage = new SendGridMessage();
                        myMessage.AddTo(email.NotifyUser.Email);
                        myMessage.From = new MailAddress(from);
                        myMessage.Subject = "Notification for Ticket #" + ticket.Id;
                        myMessage.Html = "Ticket #" + ticket.Id + " has been updated";
                        if (!string.IsNullOrWhiteSpace(ticket.AssignedId))
                        {
                            if (ticket.AssignedId == email.NotifyUserId)
                            {
                                myMessage.Html = "Ticket #" + ticket.Id + " has been assigned to you or updated";
                            }
                        }
                        var credentials = new NetworkCredential(username, password);

                        var transportWeb = new Web(credentials);

                        transportWeb.DeliverAsync(myMessage);
                    }
                }
                db.Entry(dbin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedId);
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerId);
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

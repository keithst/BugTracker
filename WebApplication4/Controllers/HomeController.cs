using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;
using WebApplication4.Models.helper;
using Microsoft.AspNet.Identity;

namespace WebApplication4.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserAccess helper = new UserAccess();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Report(string role, string type)
        {
            UserRoleHelper helperu = new UserRoleHelper();
            Reporthelper reporter = new Reporthelper();
            ReportInput rp = new ReportInput();
            var dbin = db.Tickets.ToList();
            IList<UserTicketList> access = helper.UserisOwnerorAssigned(User.Identity.GetUserId(), dbin);
            var selectlist = helperu.ListUserRoles(User.Identity.GetUserId());
            string selected = null;
            if (string.IsNullOrWhiteSpace(type))
            {
                if (string.IsNullOrWhiteSpace(role))
                {
                    IList<ReportList> reports = reporter.createreport(db.Priorities.ToList());
                    if (User.IsInRole("Admin"))
                    {
                        reporter.popreport(dbin, reports);
                        selected = "Admin";
                    }
                    if (User.IsInRole("ProjectManager"))
                    {
                        reporter.popreport(dbin, reports, access, false, false, true);
                        selected = "ProjectManager";
                    }
                    if (User.IsInRole("Developer"))
                    {
                        reporter.popreport(dbin, reports, access, false, true, false);
                        selected = "Developer";
                    }
                    if (User.IsInRole("Submitter"))
                    {
                        reporter.popreport(dbin, reports, access, true, false, false);
                        selected = "Submitter";
                    }
                    rp.reportin = reports;
                    rp.projectreportin = null;
                    rp.rolesin = new SelectList(selectlist, selected);
                    rp.type = "All";
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(role))
                {
                    if (type == "Projects")
                    {
                        IList<ProjectReportList> reports = new List<ProjectReportList>();
                        if (role == "Admin")
                        {
                            reports = reporter.createreport(db.Priorities.ToList(), db.Projects.ToList());
                        }
                        else
                        {
                            reports = reporter.createreport(db.Priorities.ToList(), db.ProjectUsers.ToList(), User.Identity.GetUserId());
                        }
                        if (role == "Admin")
                        {
                            reporter.popreport(dbin, reports);
                        }
                        if (role == "ProjectManager")
                        {
                            reporter.popreport(dbin, reports, access, false, false, true);
                        }
                        if (role == "Developer")
                        {
                            reporter.popreport(dbin, reports, access, false, true, false);
                        }
                        if (role == "Submitter")
                        {
                            reporter.popreport(dbin, reports, access, true, false, false);
                        }
                        rp.reportin = null;
                        rp.projectreportin = reports;
                        rp.rolesin = new SelectList(selectlist, role);
                        rp.type = type;
                    }
                    if (type == "All")
                    {
                        IList<ReportList> reports = reporter.createreport(db.Priorities.ToList());
                        if (role == "Admin")
                        {
                            reporter.popreport(dbin, reports);
                        }
                        if (role == "ProjectManager")
                        {
                            reporter.popreport(dbin, reports, access, false, false, true);
                        }
                        if (role == "Developer")
                        {
                            reporter.popreport(dbin, reports, access, false, true, false);
                        }
                        if (role == "Submitter")
                        {
                            reporter.popreport(dbin, reports, access, true, false, false);
                        }
                        rp.reportin = reports;
                        rp.projectreportin = null;
                        rp.rolesin = new SelectList(selectlist, role);
                        rp.type = type;
                    }
                }
            }
            return View(rp);
        }

    }
}
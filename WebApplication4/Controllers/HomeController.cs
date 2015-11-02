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

        public ActionResult Report()
        {
            Reporthelper reporter = new Reporthelper();
            var dbin = db.Tickets.ToList();
            IList<ReportList> reports = reporter.createreport(db.Priorities.ToList());
            IList<UserTicketList> access = helper.UserisOwnerorAssigned(User.Identity.GetUserId(), dbin);
            if (User.IsInRole("Admin"))
            {
                reporter.popreport(dbin, reports);
            }
            if (User.IsInRole("ProjectManager"))
            {
                reporter.popreport(dbin, reports, access, false, false, true);
            }
            if (User.IsInRole("Developer"))
            {
                reporter.popreport(dbin, reports, access, false, true, false);
            }
            if (User.IsInRole("Submitter"))
            {
                reporter.popreport(dbin, reports, access, true, false, false);
            }
            return View(reports);
        }

    }
}
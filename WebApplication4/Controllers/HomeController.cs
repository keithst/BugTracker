using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;
using WebApplication4.Models.helper;

namespace WebApplication4.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
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
            IList<ReportList> reports = reporter.createreport(db.Priorities.ToList());
            reporter.popreport(db.Tickets.ToList(), reports);
            return View(reports);
        }

    }
}
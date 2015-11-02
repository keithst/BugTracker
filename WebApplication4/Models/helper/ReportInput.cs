using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models.helper;

namespace WebApplication4.Models.helper
{
    public class ReportInput
    {
        public SelectList rolesin { get; set; }
        public string type { get; set; }
        public IList<ReportList> reportin { get; set; }
        public IList<ProjectReportList> projectreportin { get; set; }
    }
}
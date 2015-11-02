using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class ProjectReportList
    {
        public string projectname { get; set; }
        public int projecttotal { get; set; }
        public IList<ReportList> reports { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class ProjectsIn
    {
        public IList<Projects> master { get; set; }
        public IList<ProjectUsers> masterPU { get; set; }
    }
}
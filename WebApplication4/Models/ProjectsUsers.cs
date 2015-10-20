using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class ProjectsUsers
    {
        public int Id { get; set; }
        public int ProjectsId { get; set; }
        public string ProjectUserId { get; set; }

        public virtual Projects Projects { get; set; }
        public virtual ApplicationUser ProjectUser { get; set; }
    }
}
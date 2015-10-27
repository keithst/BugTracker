using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class ProjectDetails
    {
        public Projects projectin { get; set; }
        public IList<ApplicationUser> usersin { get; set; }
        public UserRoleHelper helper { get; set; }
    }
}
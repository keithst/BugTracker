using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class RoleManagerBug
    {
        public UserRoleAssignment RoleIn { get; set; }
        public IList<ApplicationUser> master { get; set; }
        public string response { get; set; }
    }
}
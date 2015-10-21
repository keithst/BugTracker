using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models.helper
{
    public class UserRoleAssignment
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList RoleInput { get; set; }
        public string[] Roles { get; set; }
    }
}
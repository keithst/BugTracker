using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication4.Models
{
    public class ProjectUsers
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectUserId { get; set; }

        public ProjectUsers()
        {
            this.Project = new HashSet<Projects>();
            this.ProjectUser = new HashSet<ApplicationUser>();
        }

        public virtual ICollection<Projects> Project { get; set; }
        public virtual ICollection<ApplicationUser> ProjectUser { get; set; }
 

    }
}
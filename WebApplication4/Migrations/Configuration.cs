namespace WebApplication4.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebApplication4.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<WebApplication4.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(WebApplication4.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "keith.sturzenbecker@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "keith.sturzenbecker@gmail.com",
                    Email = "keith.sturzenbecker@gmail.com",
                    FirstName = "Keith",
                    LastName = "Sturzenbecker"
                },
                        "sturze");
            }

            var userId = userManager.FindByEmail("keith.sturzenbecker@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");

            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }
        }
    }
}

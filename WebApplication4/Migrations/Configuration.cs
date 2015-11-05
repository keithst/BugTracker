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

            var userGuest = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "guest@guest.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "guest@guest.com",
                    Email = "guest@guest.com",
                    FirstName = "guest",
                    LastName = "guest"
                },
                        "Password-1");
            }

            var userGuestId = userManager.FindByEmail("guest@guest.com").Id;
            userManager.AddToRole(userGuestId, "Admin");

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
            if (!context.Roles.Any(r => r.Name == "Unassigned"))
            {
                roleManager.Create(new IdentityRole { Name = "Unassigned" });
            }

            if (!context.Priorities.Any(r => r.Priority == "Critical"))
            {
                context.Priorities.Add(new TicketPriority { Priority = "Critical" });
            }
            if (!context.Priorities.Any(r => r.Priority == "High"))
            {
                context.Priorities.Add(new TicketPriority { Priority = "High" });
            }
            if (!context.Priorities.Any(r => r.Priority == "Medium"))
            {
                context.Priorities.Add(new TicketPriority { Priority = "Medium" });
            }
            if (!context.Priorities.Any(r => r.Priority == "Low"))
            {
                context.Priorities.Add(new TicketPriority { Priority = "Low" });
            }

            if (!context.Status.Any(r => r.Status == "Fixed"))
            {
                context.Status.Add(new TicketStatus { Status = "Fixed" });
            }
            if (!context.Status.Any(r => r.Status == "Open"))
            {
                context.Status.Add(new TicketStatus { Status = "Open" });
            }
            if (!context.Status.Any(r => r.Status == "Assigned"))
            {
                context.Status.Add(new TicketStatus { Status = "Assigned" });
            }
            if (!context.Status.Any(r => r.Status == "Released"))
            {
                context.Status.Add(new TicketStatus { Status = "Released" });
            }
            if (!context.Status.Any(r => r.Status == "Returned"))
            {
                context.Status.Add(new TicketStatus { Status = "Returned" });
            }
            if (!context.Status.Any(r => r.Status == "NoBug"))
            {
                context.Status.Add(new TicketStatus { Status = "NoBug" });
            }

            if (!context.Types.Any(r => r.Type == "Enhancement"))
            {
                context.Types.Add(new TicketType { Type = "Enhancement" });
            }
            if (!context.Types.Any(r => r.Type == "Bug"))
            {
                context.Types.Add(new TicketType { Type = "Bug" });
            }

        }
    }
}

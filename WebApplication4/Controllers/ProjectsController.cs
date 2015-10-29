using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebApplication4.Models.helper;

namespace WebApplication4.Models
{
    [RequireHttps]
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IList<Projects> master = new List<Projects>();
        private ProjectDetails detail = new ProjectDetails();
        private Userhelp user = new Userhelp();
        private UserRoleHelper helpin = new UserRoleHelper();

        // GET: Projects
        public ActionResult Index()
        {
            if(User.IsInRole("Admin"))
            {
                master = db.Projects.ToList();
            }
            if(User.IsInRole("ProjectManager") || User.IsInRole("Developer"))
            {
                var query = db.ProjectUsers.AsQueryable();
                var queryt = db.Projects.AsQueryable();
                var idin = User.Identity.GetUserId();
                var ids = query.Where(x => x.ProjectUserId == idin).Select(x => x.ProjectId).ToList();
                foreach(var id in ids)
                {
                    master = master.Union(queryt.Where(x => x.Id == id)).ToList();
                }
            }
            return View(master);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            IList<string> tempid = db.ProjectUsers.Where(x => x.ProjectId == id).Select(y => y.ProjectUserId).ToList();
            IList<ApplicationUser> selected = new List<ApplicationUser>();
            foreach(var ids in tempid)
            {
                var query = db.Users.AsQueryable();
                selected = selected.Union(query.Where(x => x.Id == ids)).ToList();
                    
            }

            if (projects == null)
            {
                return HttpNotFound();
            }
            detail.projectin = projects;
            detail.usersin = selected;
            detail.helper = helpin;
            return View(detail);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Project")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(projects);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projects);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }

            IList<string> tempid = db.ProjectUsers.Where(x => x.ProjectId == id).Select(y => y.ProjectUserId).ToList();
            IList<string> selected = new List<string>();
            foreach (var ids in tempid)
            {
                var query = db.Users.AsQueryable();
                selected = selected.Union(query.Where(x => x.Id == ids).Select(y => y.UserName)).ToList();

            }
            if (User.IsInRole("Admin"))
            {
                var userinput = helpin.UsersInRole("ProjectManager");
                var userinput2 = helpin.UsersInRole("Developer");
                userinput = userinput.Union(userinput2).ToList();
                user.users = new MultiSelectList(userinput, "UserName", "UserName", selected);
            }
            if (User.IsInRole("ProjectManager"))
            {
                var developers = helpin.UsersInRole("Developer");
                user.users = new MultiSelectList(developers, "UserName", "UserName", selected);
            }
            user.pid = projects;

            return View(user);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Project")] Projects projects, int Id, string Project, string[] User)
        {
            if (ModelState.IsValid)
            {
                ProjectUsers projectUsers = new ProjectUsers();
                var query = db.ProjectUsers.Where(x => x.ProjectId == Id);
                foreach (var delete in query)
                {
                    db.ProjectUsers.Remove(delete);
                }
                if(User != null)
                {
                    foreach (var useradd in User)
                    {
                        var id = db.Users.Where(x => x.UserName == useradd).Select(y => y.Id).Single();
                        projectUsers.ProjectUserId = id;
                        projectUsers.ProjectId = Id;
                        db.ProjectUsers.Add(projectUsers);
                    }
                }
                projects.Id = Id;
                projects.Project = Project;
                db.Entry(projects).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projects);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Projects projects = db.Projects.Find(id);
            db.Projects.Remove(projects);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

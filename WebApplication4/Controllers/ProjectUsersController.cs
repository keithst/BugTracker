using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Models;
using WebApplication4.Models.helper;

namespace WebApplication4.Controllers
{
    public class ProjectUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private Userhelp user = new Userhelp();
        private UserRoleHelper helper = new UserRoleHelper();

        // GET: ProjectUsers
        public ActionResult Index()
        {
            return View(db.ProjectUsers.ToList());
        }

        // GET: ProjectUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectUsers projectUsers = db.ProjectUsers.Find(id);
            if (projectUsers == null)
            {
                return HttpNotFound();
            }
            return View(projectUsers);
        }

        // GET: ProjectUsers/Create
        public ActionResult Create(int id)
        {
            IList<string> tempid = db.ProjectUsers.Where(x => x.ProjectId == id).Select(y => y.ProjectUserId).ToList();
            IList<string> selected = new List<string>();
            foreach(var ids in tempid)
            {
                var query = db.Users.AsQueryable();
                selected = selected.Union(query.Where(x => x.Id == ids).Select(y => y.UserName)).ToList();
                    
            }
            if(User.IsInRole("Admin"))
            {
                var userinput = helper.UsersInRole("ProjectManager");
                var userinput2 = helper.UsersInRole("Developer");
                userinput = userinput.Union(userinput2).ToList();
                user.users = new MultiSelectList(userinput, "UserName", "UserName", selected);
            }
            if(User.IsInRole("ProjectManager"))
            {
                var developers = helper.UsersInRole("Developer");
                user.users = new MultiSelectList(developers, "UserName", "UserName", selected);
            }
            return View(user);
        }

        // POST: ProjectUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProjectId,ProjectUserId")] ProjectUsers projectUsers, string[] User)
        {
            if (ModelState.IsValid)
            {
                foreach(var useradd in User)
                {
                    var id = db.Users.Where(x => x.UserName == useradd).Select(y => y.Id).Single();
                    projectUsers.ProjectUserId = id;
                    db.ProjectUsers.Add(projectUsers);
                }
                db.SaveChanges();
                return RedirectToAction("Details", "Projects", new { id = projectUsers.ProjectId });
            }

            return View(projectUsers);
        }

        // GET: ProjectUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectUsers projectUsers = db.ProjectUsers.Find(id);
            if (projectUsers == null)
            {
                return HttpNotFound();
            }
            return View(projectUsers);
        }

        // POST: ProjectUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProjectId,ProjectUserId")] ProjectUsers projectUsers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectUsers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projectUsers);
        }

        // GET: ProjectUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectUsers projectUsers = db.ProjectUsers.Find(id);
            if (projectUsers == null)
            {
                return HttpNotFound();
            }
            return View(projectUsers);
        }

        // POST: ProjectUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectUsers projectUsers = db.ProjectUsers.Find(id);
            db.ProjectUsers.Remove(projectUsers);
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

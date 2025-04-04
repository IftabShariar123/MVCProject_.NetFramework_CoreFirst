using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Project_MVC_CF_Special.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_MVC_CF_Special.Controllers
{
    public class RolesController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        // GET: Roles
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            // Populate Dropdown Lists
            var context = new Models.ApplicationDbContext();

            var rolelist = context.Roles.OrderBy(r => r.Name).ToList().Select(rr =>
            new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = rolelist;

            var userlist = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
            new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
            ViewBag.Users = userlist;

            ViewBag.Message = "";

            return View();
        }

        // GET: /Roles/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Roles/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                var context = new Models.ApplicationDbContext();
                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {
                    Name = collection["RoleName"]
                });
                context.SaveChanges();
                ViewBag.Message = "Role created successfully !";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }






        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string RoleName)
        {
            var context = new Models.ApplicationDbContext();
            var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Roles.Remove(thisRole);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Roles/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string roleName)
        {
            var context = new Models.ApplicationDbContext();
            var thisRole = context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            return View(thisRole);
        }

        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                var context = new Models.ApplicationDbContext();
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult RoleAddToUser()
        {
            var context = new Models.ApplicationDbContext();

            // Populate Dropdown Lists
            var rolelist = context.Roles.OrderBy(r => r.Name).ToList()
                .Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = rolelist;

            var userlist = context.Users.OrderBy(u => u.UserName).ToList()
                .Select(uu => new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
            ViewBag.Users = userlist;

            return View();
        }







        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName)
        {
            var context = new Models.ApplicationDbContext();

            // Ensure that context is not null (though it's very unlikely in this case)
            if (context == null)
            {
                throw new ArgumentNullException("context", "Context must not be null.");
            }

            // Try to fetch the user from the database based on the provided UserName
            ApplicationUser user = context.Users
                .Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefault();

            // If the user doesn't exist, show an appropriate message and return to the view
            if (user == null)
            {
                ViewBag.Message = "User not found!";
            }
            else
            {
                // Ensure the user is not already in the role before adding the role
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                if (!userManager.IsInRole(user.Id, RoleName))
                {
                    userManager.AddToRole(user.Id, RoleName);
                    ViewBag.Message = "Role added successfully!";
                }
                else
                {
                    ViewBag.Message = "User already has this role.";
                }
            }

            // Repopulate Dropdown Lists for User and Roles after adding the role
            var rolelist = context.Roles.OrderBy(r => r.Name).ToList()
                .Select(rr => new SelectListItem { Value = rr.Name, Text = rr.Name }).ToList();
            ViewBag.Roles = rolelist;

            var userlist = context.Users.OrderBy(u => u.UserName).ToList()
                .Select(uu => new SelectListItem { Value = uu.UserName, Text = uu.UserName }).ToList();
            ViewBag.Users = userlist;

            // Clear the selected values
            ViewBag.SelectedUser = string.Empty;
            ViewBag.SelectedRole = string.Empty;

            // Return the view with the updated message and dropdowns
            return View();
        }






        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                var context = new Models.ApplicationDbContext();
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                ViewBag.RolesForThisUser = userManager.GetRoles(user.Id);

                // Repopulate Dropdown Lists
                var rolelist = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = rolelist;
                var userlist = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
                new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
                ViewBag.Users = userlist;

                ViewBag.Message = "Roles retrieved successfully !";
            }

            return View("RoleAddToUser");
        }







        //Deleting a User from A Role
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            var account = new AccountController();
            var context = new Models.ApplicationDbContext();
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (userManager.IsInRole(user.Id, RoleName))
            {
                userManager.RemoveFromRole(user.Id, RoleName);
                ViewBag.Message = "Role removed from this user successfully !";
            }
            else
            {
                ViewBag.Message = "This user doesn't belong to selected role.";
            }

            // Repopulate Dropdown Lists
            var rolelist = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = rolelist;
            var userlist = context.Users.OrderBy(u => u.UserName).ToList().Select(uu =>
            new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
            ViewBag.Users = userlist;

            return View("RoleAddToUser");
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRolePermissions(string roleName, List<string> permissions)
        {
            if (!string.IsNullOrEmpty(roleName) && permissions != null)
            {
                var existingPermissions = db.RolePermissions.Where(rp => rp.RoleName == roleName);
                db.RolePermissions.RemoveRange(existingPermissions);

                foreach (var permission in permissions)
                {
                    db.RolePermissions.Add(new RolePermission { RoleName = roleName, Permission = permission });
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
     
        public JsonResult GetRolePermissions(string roleName)
        {
            var permissions = db.RolePermissions
                                .Where(rp => rp.RoleName == roleName)
                                .Select(rp => rp.Permission)
                                .ToList();

            return Json(permissions, JsonRequestBehavior.AllowGet);
        }



    }
}
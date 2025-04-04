using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Project_MVC_CF_Special.Models;

namespace Project_MVC_CF_Special.Controllers
{
    public class CategoryController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        public JsonResult Details(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return Json(new { error = "Not found" }, JsonRequestBehavior.AllowGet);
            }
            return Json(category, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Edit(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return Json(new { error = "Not found" }, JsonRequestBehavior.AllowGet);
            }
            return Json(category, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        public JsonResult Delete(int id)
        {
            var category = db.Categories.Find(id);
            if (category == null)
            {
                return Json(new { error = "Not found" }, JsonRequestBehavior.AllowGet);
            }
            return Json(category, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}
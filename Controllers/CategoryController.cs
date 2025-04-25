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
            return View(db.GetAllCategories());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                // Using stored procedure
                var newId = db.CreateCategory(category.CategoryName);
                return Json(new { success = true, id = newId });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        public JsonResult Details(int id)
        {
            var category = db.GetCategoryById(id);
            if (category == null)
            {
                return Json(new { error = "Not found" }, JsonRequestBehavior.AllowGet);
            }
            return Json(category, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Edit(int id)
        {
            var category = db.GetCategoryById(id);
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
                // Using stored procedure
                db.UpdateCategory(category.CategoryID, category.CategoryName);
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        public JsonResult Delete(int id)
        {
            var category = db.GetCategoryById(id);
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
            // Using stored procedure
            db.DeleteCategory(id);
            return Json(new { success = true });
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
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
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View(db.Products.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

       

        public JsonResult Details(int id)
        {
            var product = db.Products
                .Include(p => p.Category) // Include the Category data
                .Where(p => p.ProductID == id)
                .Select(p => new {
                    p.ProductID,
                    p.ProductName,
                    CategoryName = p.Category.CategoryName // Get the CategoryName
                })
                .FirstOrDefault();

            if (product == null)
            {
                return Json(new { error = "Not found" }, JsonRequestBehavior.AllowGet);
            }
            return Json(product, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Edit(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return Json(new { error = "Not found" }, JsonRequestBehavior.AllowGet);
            }

            // Get all categories for the dropdown
            var categories = db.Categories.Select(c => new {
                Value = c.CategoryID,
                Text = c.CategoryName
            }).ToList();

            return Json(new
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                CategoryID = product.CategoryID,
                Categories = categories
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });
        }

        public JsonResult Delete(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return Json(new { error = "Not found" }, JsonRequestBehavior.AllowGet);
            }
            return Json(product, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return Json(new { success = true });
        }
    }
}
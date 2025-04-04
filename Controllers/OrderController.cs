using Project_MVC_CF_Special.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Security.Claims;
using System.Net.Mail;
using PagedList;
namespace Project_MVC_CF_Special.Controllers
{
    public class OrderController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        private bool HasPermission(string permission)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            if (claimsIdentity == null) return false;

            var userRoles = claimsIdentity.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return db.RolePermissions.Any(rp => userRoles.Contains(rp.RoleName) && rp.Permission == permission);
        }


        public ActionResult Index()
        {

            if (!HasPermission("Index"))
            {
                return RedirectToAction("UnauthorizedAccess");
            }


            return View();
        }

        public JsonResult getProductCategories()
        {
            List<Category> categories = new List<Category>();
            categories = db.Categories.OrderBy(a => a.CategoryName).ToList();
            return new JsonResult { Data = categories, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult getProducts(int categoryID)
        {
            List<Product> products = new List<Product>();
            products = db.Products.Where(a => a.CategoryID.Equals(categoryID)).OrderBy(a => a.ProductName).ToList();
            return new JsonResult { Data = products, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }



        [HttpPost]
        public JsonResult save(OrderMaster order, HttpPostedFileBase file)
        {
            bool status = false;

            if (file != null)
            {
                string folderPath = Server.MapPath("~/Images/");

                // Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(folderPath, fileName);
                file.SaveAs(filePath);
                order.AddressProofImage = fileName;
            }

            // Validate ModelState instead of TryUpdateModel
            if (ModelState.IsValid)
            {
                db.OrderMasters.Add(order);
                db.SaveChanges();
                status = true;


                string customerName = order.Description; // Order er CustomerName property theke name pabe
                int orderId = order.OrderID;
                string emailBody = $"Order No-{orderId} <br /> <br /> Dear {customerName}, your order has been successfully placed.<br /> <br /> Thank You for shopping with us.";

                SendEmail(order.EmailAddress, "Order Confirmation", emailBody);

            }

            return Json(new { status = status });
        }



        private void SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                string customerName = body; // assuming body will hold customer name as well

                string emailBody = $"Dear {customerName}, your order has been successfully placed.";


                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("iftabshariar1234@gmail.com"); // Change to your email
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.Credentials = new NetworkCredential("iftabshariar1234@gmail.com", "jrhx ohja nabm djwo"); // Update with your email & password
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                // Log the exception or handle error
                Console.WriteLine("Email Sending Failed: " + ex.Message);
            }
        }





        [HttpGet]
        public ActionResult Edit(int? id)
        {

            if (!HasPermission("Update"))
            {
                return RedirectToAction("UnauthorizedAccess");
            }



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // OrderDetails সহ OrderMaster লোড করুন
            OrderMaster orderMaster = db.OrderMasters
                .Include(o => o.OrderDetails)
                .Include(o => o.OrderDetails.Select(od => od.product))
                .SingleOrDefault(o => o.OrderID == id);

            if (orderMaster == null)
            {
                return HttpNotFound();
            }

            return View(orderMaster);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OrderMaster orderMaster, List<int> RemovedOrderDetailIDs)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. অর্ডার মাষ্টার আপডেট করুন
                        var existingOrder = db.OrderMasters
                            .Include(o => o.OrderDetails)
                            .FirstOrDefault(o => o.OrderID == orderMaster.OrderID);

                        if (existingOrder == null)
                        {
                            return HttpNotFound();
                        }

                        // মাষ্টার ডেটা আপডেট করুন
                        db.Entry(existingOrder).CurrentValues.SetValues(orderMaster);

                        // 2. ডিলিট হওয়া আইটেমগুলো হ্যান্ডেল করুন
                        if (RemovedOrderDetailIDs != null)
                        {
                            foreach (var id in RemovedOrderDetailIDs)
                            {
                                var detail = db.OrderDetails.Find(id);
                                if (detail != null)
                                {
                                    db.OrderDetails.Remove(detail);
                                }
                            }
                        }

                        // 3. বিদ্যমান আইটেম আপডেট করুন
                        foreach (var detail in orderMaster.OrderDetails)
                        {
                            var existingDetail = existingOrder.OrderDetails
                                .FirstOrDefault(od => od.OrderDetailID == detail.OrderDetailID);

                            if (existingDetail != null)
                            {
                                // শুধুমাত্র Quantity এবং Rate আপডেট করুন
                                existingDetail.Quantity = detail.Quantity;
                                existingDetail.Rate = detail.Rate;
                            }
                        }

                        db.SaveChanges();
                        transaction.Commit();
                        return RedirectToAction("List");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Error saving changes: " + ex.Message);

                        // ডাটা রিলোড করুন
                        orderMaster.OrderDetails = db.OrderDetails
                            .Include(od => od.product)
                            .Where(od => od.OrderID == orderMaster.OrderID)
                            .ToList();
                    }
                }
            }

            return View(orderMaster);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }






        [Route("Details")]
        public ActionResult List(int? page, int? pageSize, string searchTerm, string filterField,
                        DateTime? fromDate, DateTime? toDate, string sortField = "OrderID",
                        string sortDirection = "asc")
        {
            // Set default values
            int defaultPageSize = 10;
            int currentPageSize = pageSize ?? defaultPageSize;
            int pageNumber = (page ?? 1);

            // Start with all orders
            var orders = db.OrderMasters.AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (filterField == "OrderID")
                {
                    if (int.TryParse(searchTerm, out int orderId))
                    {
                        orders = orders.Where(o => o.OrderID == orderId);
                    }
                }
                else if (filterField == "CustomerName")
                {
                    orders = orders.Where(o => o.Description.Contains(searchTerm));
                }
                else if (filterField == "Email")
                {
                    orders = orders.Where(o => o.EmailAddress.Contains(searchTerm));
                }
            }

            // Date range filter
            if (fromDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate <= toDate.Value);
            }

            // Apply sorting
            switch (sortField)
            {
                case "OrderID":
                    orders = sortDirection == "asc" ?
                        orders.OrderBy(o => o.OrderID) :
                        orders.OrderByDescending(o => o.OrderID);
                    break;
                case "OrderDate":
                    orders = sortDirection == "asc" ?
                        orders.OrderBy(o => o.OrderDate) :
                        orders.OrderByDescending(o => o.OrderDate);
                    break;
                case "CustomerName":
                    orders = sortDirection == "asc" ?
                        orders.OrderBy(o => o.Description) :
                        orders.OrderByDescending(o => o.Description);
                    break;
                case "Email":
                    orders = sortDirection == "asc" ?
                        orders.OrderBy(o => o.EmailAddress) :
                        orders.OrderByDescending(o => o.EmailAddress);
                    break;
                default:
                    orders = orders.OrderBy(o => o.OrderID);
                    break;
            }

            // Store values in ViewBag
            ViewBag.PageSize = currentPageSize;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.FilterField = filterField;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            ViewBag.SortField = sortField;
            ViewBag.SortDirection = sortDirection;

            // Return paginated list
            var pagedOrders = orders.ToPagedList(pageNumber, currentPageSize);
            return View(pagedOrders);
        }




        [HttpPost]
        public JsonResult Delete(int id)
        {

            if (!HasPermission("Delete"))
            {
                return Json(new { success = false, message = "You do not have permission to delete orders." });
            }


            var order = db.OrderMasters.Find(id);
            if (order != null)
            {
                db.OrderMasters.Remove(order);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Order not found." });
        }

        public ActionResult UnauthorizedAccess()
        {
            return View();
        }

    }
}
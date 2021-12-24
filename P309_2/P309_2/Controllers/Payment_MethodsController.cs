using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using PagedList;
using System.Web;
using System.Web.Mvc;
using P309_2.Models;

namespace P309_2.Controllers
{
    public class Payment_MethodsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Payment_Methods
        [Authorize]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
 
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString; 

            var Method = from s in db.Payment_Method  
                       select s;
             
            if (!String.IsNullOrEmpty(searchString))
            {
                Method = Method.Where(s => s.Payment_Method.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString; 

            switch (sortOrder)
            {
                case "Name_desc":
                    Method = Method.OrderByDescending(s => s.Payment_Method);
                    break;
                default:  
                    Method = Method.OrderBy(s => s.Payment_Method);
                    break;
            }

            int pageSize = 7;  
            int pageNumber = (page ?? 1);   
            return View(Method.ToPagedList(pageNumber, pageSize)); 
        }

        // GET: Payment_Methods/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Payment_Methods/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Payment_Method")] Payment_Methods payment_Methods)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var ExistingName = db.Payment_Method.Where(a => a.Payment_Method == payment_Methods.Payment_Method).First();    
                    ViewBag.ExistingName = "Stage already exists";  
                    return View(payment_Methods); 
                }
                catch (Exception)  
                {
                    db.Payment_Method.Add(payment_Methods);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
             
            }

            return View(payment_Methods);
        }

        public ActionResult Back_to_company_creation()
        {
            return RedirectToAction("Create", "Companies");
        }

        // GET: Payment_Methods/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Payment_Methods payment_Methods = db.Payment_Method.Find(id);

            if (payment_Methods == null)
            {
                return HttpNotFound();
            }
            return View(payment_Methods);
        }

        // POST: Payment_Methods/Edit
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Payment_Method")] Payment_Methods payment_Methods)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var ExistingName = db.Payment_Method.Where(a => a.Payment_Method == payment_Methods.Payment_Method).First();  
                    ViewBag.ExistingName = "Stage already exists";  
                    return View(payment_Methods); 
                }
                catch (Exception)  
                {
                    db.Payment_Method.Add(payment_Methods);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(payment_Methods);
        }

        // GET: Payment_Methods/Delete
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Payment_Methods payment_Methods = db.Payment_Method.Find(id);

            if (payment_Methods == null)
            {
                return HttpNotFound();
            }
            return View(payment_Methods);
        }

        // POST: Payment_Methods/Delete
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payment_Methods payment_Methods = db.Payment_Method.Find(id);
            db.Payment_Method.Remove(payment_Methods);
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

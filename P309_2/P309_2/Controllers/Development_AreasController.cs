using System;
using System.Collections.Generic;
using System.Data;
using PagedList;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using P309_2.Models;

namespace P309_2.Controllers
{
    public class Development_AreasController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Development_Areas
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

            var Area = from s in db.Development_Area 
                          select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                Area = Area.Where(s => s.Development_Area.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString;

            switch (sortOrder)
            {
                case "Name_desc":
                    Area = Area.OrderByDescending(s => s.Development_Area);
                    break;
                default: 
                    Area = Area.OrderBy(s => s.Development_Area);
                    break;
            }
 
            int pageSize = 7;  
            int pageNumber = (page ?? 1); 
            return View(Area.ToPagedList(pageNumber, pageSize)); 
        }

        // GET: Development_Areas/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Development_Areas/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Development_Area")] Development_Areas development_Areas)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    var ExistingName = db.Development_Area.Where(a => a.Development_Area == development_Areas.Development_Area).First(); 
                    ViewBag.ExistingName = "Area already exists"; 
                    return View(development_Areas); 
                }
                catch (Exception) 
                {
                    db.Development_Area.Add(development_Areas);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
              
            }
            return View(development_Areas);
        }

        public ActionResult Back_to_project_creation()
        {
            return RedirectToAction("Create", "Projects");
        }

        // GET: Development_Areas/Edit
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Development_Areas development_Areas = db.Development_Area.Find(id);

            if (development_Areas == null)
            {
                return HttpNotFound();
            }

            return View(development_Areas);
        }

        // POST: Development_Areas/Edit
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Development_Area")] Development_Areas development_Areas)
        {
            var originalData = db.Development_Area.AsNoTracking().Where(s => s.Id == development_Areas.Id).FirstOrDefault();

            if (ModelState.IsValid)
            {
                try
                {
                    if (originalData.Development_Area != development_Areas.Development_Area)
                    {
                        var ExistingName = db.Development_Area.Where(a => a.Development_Area == development_Areas.Development_Area).First(); 
                    }
                    else
                    {
                        var ExistingName = db.Development_Area.Where(a => a.Development_Area == "uhw22yde27e2s3882s").First(); 
                    }

                    ViewBag.ExistingName = "Area already exists"; 
                    return View(development_Areas); 
                }
                catch (Exception) 
                {
                    db.Entry(development_Areas).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(development_Areas);
        }

        // GET: Development_Areas/Delete
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Development_Areas development_Areas = db.Development_Area.Find(id);

            if (development_Areas == null)
            {
                return HttpNotFound();
            }
            return View(development_Areas);
        }

        // POST: Development_Areas/Delete
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Development_Areas development_Areas = db.Development_Area.Find(id);
            db.Development_Area.Remove(development_Areas);
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

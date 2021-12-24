using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using PagedList;
using System.Net;
using System.Web;
using System.Web.Mvc;
using P309_2.Models;

namespace P309_2.Controllers
{
    public class Development_StagesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Development_Stages
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

            var Stage = from s in db.Development_Stage 
                       select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                Stage = Stage.Where(s => s.Development_Stage.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString;

            switch (sortOrder)
            {
                case "Name_desc":
                    Stage = Stage.OrderByDescending(s => s.Development_Stage);
                    break;
                default: 
                    Stage = Stage.OrderBy(s => s.Development_Stage);
                    break;
            }

            int pageSize = 7;  
            int pageNumber = (page ?? 1); 
            return View(Stage.ToPagedList(pageNumber, pageSize)); 
        }

        // GET: Development_Stages/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Development_Stages/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Development_Stage")] Development_Stages development_Stages)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var ExistingName = db.Development_Stage.Where(a => a.Development_Stage == development_Stages.Development_Stage).First();  
                    ViewBag.ExistingName = "Stage already exists";  
                    return View(development_Stages); 
                }
                catch (Exception)  
                {
                    db.Development_Stage.Add(development_Stages);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

            }

            return View(development_Stages);
        }

        public ActionResult Back_to_project_creation()
        {
            return RedirectToAction("Create", "Projects");
        }

        // GET: Development_Stages/Edit
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Development_Stages development_Stages = db.Development_Stage.Find(id);

            if (development_Stages == null)
            {
                return HttpNotFound();
            }

            return View(development_Stages);
        }

        // POST: Development_Stages/Edit
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Development_Stage")] Development_Stages development_Stages)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var ExistingName = db.Development_Stage.Where(a => a.Development_Stage == development_Stages.Development_Stage).First();  
                    ViewBag.ExistingName = "Stage already exists";  
                    return View(development_Stages);  
                }
                catch (Exception) 
                {
                    db.Development_Stage.Add(development_Stages);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(development_Stages);
        }

        // GET: Development_Stages/Delete
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Development_Stages development_Stages = db.Development_Stage.Find(id);

            if (development_Stages == null)
            {
                return HttpNotFound();
            }
            return View(development_Stages);
        }

        // POST: Development_Stages/Delete
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Development_Stages development_Stages = db.Development_Stage.Find(id);
            db.Development_Stage.Remove(development_Stages);
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

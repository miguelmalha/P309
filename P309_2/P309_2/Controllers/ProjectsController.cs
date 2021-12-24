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
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.ProjNumberSortParm = sortOrder == "ProjNumber" ? "ProjNumber_desc" : "ProjNumber";
            ViewBag.ObservationsSortParm = sortOrder == "Observations" ? "Observations_desc" : "Observations";
            ViewBag.AreaSortParm = sortOrder == "Area" ? "Area_desc" : "Area";
            ViewBag.StageSortParm = sortOrder == "Stage" ? "Stage_desc" : "Stage";
            ViewBag.CompanySortParm = sortOrder == "Company" ? "Company_desc" : "Company";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var projects = from s in db.Project
                           select s;

            projects = db.Project.Include(p => p.Company).Include(p => p.Development_Area).Include(p => p.Development_Stage);


            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(s => s.Name.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString;

            switch (sortOrder)
            {
                case "Name_desc":
                    projects = projects.OrderByDescending(s => s.Name);
                    break;
                case "ProjNumber":
                    projects = projects.OrderBy(s => s.ProjectNumber);
                    break;
                case "ProjNumber_desc":
                    projects = projects.OrderByDescending(s => s.ProjectNumber);
                    break;
                case "Observations":
                    projects = projects.OrderBy(s => s.Observations);
                    break;
                case "Observations_desc":
                    projects = projects.OrderByDescending(s => s.Observations);
                    break;
                case "Area":
                    projects = projects.OrderBy(s => s.Development_Area.Development_Area);
                    break;
                case "Area_desc":
                    projects = projects.OrderByDescending(s => s.Development_Area.Development_Area);
                    break;
                case "Stage":
                    projects = projects.OrderBy(s => s.Development_Stage.Development_Stage);
                    break;
                case "Stage_desc":
                    projects = projects.OrderByDescending(s => s.Development_Stage.Development_Stage);
                    break;
                case "Company":
                    projects = projects.OrderBy(s => s.Company.Name);
                    break;
                case "Company_desc":
                    projects = projects.OrderByDescending(s => s.Company.Name);
                    break;
                default:   
                    projects = projects.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 7;  
            int pageNumber = (page ?? 1); 
            return View(projects.ToPagedList(pageNumber, pageSize)); 
        }

        // GET: Projects/Details
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Projects projects = db.Project.Find(id);
            projects.Development_Stage = db.Development_Stage.Where(a => a.Id == projects.Development_Stage_Id).First();
            projects.Development_Area = db.Development_Area.Where(a => a.Id == projects.Development_Area_Id).First();
            projects.Company = db.Company.Where(a => a.Id == projects.Company_Id).First();
            projects.Logs = db.Project_Log.Where(a => a.Project_Id == projects.ProjectNumber).ToList();

            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // GET: Projects/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name");
            ViewBag.Development_Area_Id = new SelectList(db.Development_Area, "Id", "Development_Area");
            ViewBag.Development_Stage_Id = new SelectList(db.Development_Stage, "Id", "Development_Stage");
            return View();
        }

        // POST: Projects/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProjectNumber,Name,Company_Id,Development_Stage_Id,Development_Area_Id,Observations")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    if (projects.Company_Id == 0)
                    {
                        ViewBag.NoCompany = "Add a company first";
                        ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", projects.Company_Id);
                        ViewBag.Development_Area_Id = new SelectList(db.Development_Area, "Id", "Development_Area", projects.Development_Area_Id);
                        ViewBag.Development_Stage_Id = new SelectList(db.Development_Stage, "Id", "Development_Stage", projects.Development_Stage_Id);
                        return View(projects);
                    }

                    db.Project.Add(projects);
                    db.SaveChanges();
                    projects.Company = db.Company.Where(a => a.Id == projects.Company_Id).First();
                    projects.Development_Stage = db.Development_Stage.Where(a => a.Id == projects.Development_Stage_Id).First();
                    projects.Development_Area = db.Development_Area.Where(a => a.Id == projects.Development_Area_Id).First();

                    Project_Logs log = new Project_Logs
                    {
                        Project_Id = projects.ProjectNumber,
                        UserId = User.Identity.Name,
                        Created = DateTime.Now,
                        Description = "(Project Created)\t" + "\t\tName:\t" + projects.Name + ",\tProject to:\t" + projects.Company.Name + ",\tProject Area:\t" + projects.Development_Area.Development_Area + ",\tCurrent Stage:\t" + projects.Development_Stage.Development_Stage + ",\tObservations:\t" + projects.Observations
                    };

                    db.Project_Log.Add(log);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", projects.Company_Id);
                    ViewBag.Development_Area_Id = new SelectList(db.Development_Area, "Id", "Development_Area", projects.Development_Area_Id);
                    ViewBag.Development_Stage_Id = new SelectList(db.Development_Stage, "Id", "Development_Stage", projects.Development_Stage_Id);
                    return View(projects);
                }
            }

            ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", projects.Company_Id);
            ViewBag.Development_Area_Id = new SelectList(db.Development_Area, "Id", "Development_Area", projects.Development_Area_Id);
            ViewBag.Development_Stage_Id = new SelectList(db.Development_Stage, "Id", "Development_Stage", projects.Development_Stage_Id);
            return View(projects);
        }

        [Authorize]
        public ActionResult Development_Areas()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Development_Areas");
            }
            return View("Error");
        }

        [Authorize]
        public ActionResult Development_Stages()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Development_Stages");
            }
            return View("Error");
        }

        // GET: Projects/Edit
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Projects projects = db.Project.Find(id);

            if (projects == null)
            {
                return HttpNotFound();
            }

            ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", projects.Company_Id);
            ViewBag.Development_Area_Id = new SelectList(db.Development_Area, "Id", "Development_Area", projects.Development_Area_Id);
            ViewBag.Development_Stage_Id = new SelectList(db.Development_Stage, "Id", "Development_Stage", projects.Development_Stage_Id);
            return View(projects);
        }

        // POST: Projects/Edit
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProjectNumber,Name,Company_Id,Development_Stage_Id,Development_Area_Id,Observations")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(projects).State = EntityState.Modified;
                    db.SaveChanges();

                    projects.Company = db.Company.Where(a => a.Id == projects.Company_Id).First();
                    projects.Development_Stage = db.Development_Stage.Where(a => a.Id == projects.Development_Stage_Id).First();
                    projects.Development_Area = db.Development_Area.Where(a => a.Id == projects.Development_Area_Id).First();             

                    Project_Logs log = new Project_Logs
                    {
                        Project_Id = projects.ProjectNumber,
                        UserId = User.Identity.Name,
                        Created = DateTime.Now,
                        Description = "(Project Updated)\t" + "\t\tName:\t" + projects.Name + ",\tProject to:\t" + projects.Company.Name + ",\tProject Area:\t" + projects.Development_Area.Development_Area + ",\tCurrent Stage:\t" + projects.Development_Stage.Development_Stage +  ",\tObservations:\t" + projects.Observations 
                    };

                    db.Project_Log.Add(log);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", projects.Company_Id);
                    ViewBag.Development_Area_Id = new SelectList(db.Development_Area, "Id", "Development_Area", projects.Development_Area_Id);
                    ViewBag.Development_Stage_Id = new SelectList(db.Development_Stage, "Id", "Development_Stage", projects.Development_Stage_Id);
                    return View(projects);
                }
            }

            ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", projects.Company_Id);
            ViewBag.Development_Area_Id = new SelectList(db.Development_Area, "Id", "Development_Area", projects.Development_Area_Id);
            ViewBag.Development_Stage_Id = new SelectList(db.Development_Stage, "Id", "Development_Stage", projects.Development_Stage_Id);
            return View(projects);
        }

        // GET: Projects/Delete
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Projects projects = db.Project.Find(id);
            projects.Development_Stage = db.Development_Stage.Where(a => a.Id == projects.Development_Stage_Id).First();
            projects.Development_Area = db.Development_Area.Where(a => a.Id == projects.Development_Area_Id).First();
            projects.Company = db.Company.Where(a => a.Id == projects.Company_Id).First();

            if (projects == null)
            {
                return HttpNotFound();
            }

            return View(projects);
        }

        // POST: Projects/Delete
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Projects projects = db.Project.Find(id);
            db.Project.Remove(projects);
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

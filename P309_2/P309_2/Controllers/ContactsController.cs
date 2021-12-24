using P309_2.Models;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace P309_2.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contacts
        [Authorize]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "Email_desc" : "Email";
            ViewBag.PhoneSortParm = sortOrder == "Phone" ? "Phone_desc" : "Phone";
            ViewBag.AddressSortParm = sortOrder == "Address" ? "Address_desc" : "Address";
            ViewBag.LocationSortParm = sortOrder == "Location" ? "Location_desc" : "Location";
            ViewBag.ZipCodeSortParm = sortOrder == "ZipCode" ? "ZipCode_desc" : "ZipCode";
            ViewBag.CountrySortParm = sortOrder == "Country" ? "Country_desc" : "Country";
            ViewBag.UserSortParm = sortOrder == "User" ? "User_desc" : "User";
            ViewBag.ObservationsSortParm = sortOrder == "Observations" ? "Observations_desc" : "Observations";
            ViewBag.CreatedSortParm = sortOrder == "Created" ? "Created_desc" : "Created";
            ViewBag.UpdatedSortParm = sortOrder == "Updated" ? "Updated_desc" : "Updated";
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

            var contacts = from s in db.Contact
                           select s;

            contacts = db.Contact.Include(c => c.Company);

            if (!String.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(s => s.Name.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString;

            switch (sortOrder)
            {
                case "Name_desc":
                    contacts = contacts.OrderByDescending(s => s.Name);
                    break;
                case "Email":
                    contacts = contacts.OrderBy(s => s.Email);
                    break;
                case "Email_desc":
                    contacts = contacts.OrderByDescending(s => s.Email);
                    break;
                case "Phone":
                    contacts = contacts.OrderBy(s => s.PhoneNumber);
                    break;
                case "Phone_desc":
                    contacts = contacts.OrderByDescending(s => s.PhoneNumber);
                    break;
                case "Created":
                    contacts = contacts.OrderBy(s => s.Created);
                    break;
                case "Created_desc":
                    contacts = contacts.OrderByDescending(s => s.Created);
                    break;
                case "Updated":
                    contacts = contacts.OrderBy(s => s.Updated);
                    break;
                case "Updated_desc":
                    contacts = contacts.OrderByDescending(s => s.Updated);
                    break;
                case "Company":
                    contacts = contacts.OrderBy(s => s.Company.Name);
                    break;
                case "Company_desc":
                    contacts = contacts.OrderByDescending(s => s.Company.Name);
                    break;
                case "Location":
                    contacts = contacts.OrderBy(s => s.Location);
                    break;
                case "Location_desc":
                    contacts = contacts.OrderByDescending(s => s.Location);
                    break;
                case "User":
                    contacts = contacts.OrderBy(s => s.UserId);
                    break;
                case "User_desc":
                    contacts = contacts.OrderByDescending(s => s.UserId);
                    break;
                case "ZipCode":
                    contacts = contacts.OrderBy(s => s.ZIP_Code);
                    break;
                case "ZipCode_desc":
                    contacts = contacts.OrderByDescending(s => s.ZIP_Code);
                    break;
                case "Country":
                    contacts = contacts.OrderBy(s => s.Country);
                    break;
                case "Country_desc":
                    contacts = contacts.OrderByDescending(s => s.Country);
                    break;
                case "Observations":
                    contacts = contacts.OrderBy(s => s.Observations);
                    break;
                case "Observations_desc":
                    contacts = contacts.OrderByDescending(s => s.Observations);
                    break;
                default:  
                    contacts = contacts.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 7;
            int pageNumber = (page ?? 1);
            return View(contacts.ToPagedList(pageNumber, pageSize));
        }

        // GET: Contacts/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contacts contacts = db.Contact.Find(id);

            contacts.Company = db.Company.Where(a => a.Id == contacts.Company_Id).First();

            if (contacts == null)
            {
                return HttpNotFound();
            }
            return View(contacts);
        }

        // GET: Contacts/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name");
            return View();
        }

        // POST: Contacts/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,PhoneNumber,Address,Location,ZIP_Code,Country,Company_Id,Observations,UserId,Created,Updated")] Contacts contacts)
        {
            ViewBag.ExistingEmail = null;
            ViewBag.ExistingPhone = null;
            ViewBag.NoCompany = null;

            if (ModelState.IsValid)
            {
                try
                {
                    if (contacts.Company_Id == 0)
                    {
                        ViewBag.NoCompany = "Add a company first";
                        ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", contacts.Company_Id);
                        return View(contacts);
                    }

                    var ExistingEmail = db.Contact.Where(a => a.Email != null && a.Email == contacts.Email).First();
                    ViewBag.ExistingEmail = "Email already taken"; 
                    ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", contacts.Company_Id);
                    return View(contacts);
                }
                catch (Exception)
                {
                    try
                    {
                        var ExistingPhone = db.Contact.Where(a => a.PhoneNumber != null && a.PhoneNumber == contacts.PhoneNumber).First();
                        ViewBag.ExistingPhone = "Phone Number already taken"; 
                        ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", contacts.Company_Id);
                        return View(contacts);
                    }
                    catch (Exception)
                    {
                        contacts.Created = DateTime.Now;
                        contacts.Updated = DateTime.Now;
                        contacts.UserId = User.Identity.Name;
                        db.Contact.Add(contacts);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
            }

            ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", contacts.Company_Id);
            return View(contacts);
        }

        // GET: Contacts/Edit
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contacts contacts = db.Contact.Find(id);

            if (contacts == null)
            {
                return HttpNotFound();
            }

            ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", contacts.Company_Id);
            return View(contacts);
        }

        // POST: Contacts/Edit
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,PhoneNumber,Address,Location,ZIP_Code,Country,Company_Id,Observations,UserId,Created,Updated")] Contacts contacts)
        {
            var originalData = db.Contact.AsNoTracking().Where(s => s.Id == contacts.Id).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (originalData.Email != contacts.Email)
                {
                    try
                    {
                        var ExistingEmail = db.Contact.Where(a => a.Email != null && a.Email == contacts.Email).First();
                        ViewBag.ExistingEmail = "Email already taken";
                        ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", contacts.Company_Id);
                        return View(contacts);
                    }
                    catch
                    {
                        if (originalData.PhoneNumber != contacts.PhoneNumber)
                        {
                            try
                            {
                                var ExistingPhone = db.Contact.Where(a => a.PhoneNumber != null && a.PhoneNumber == contacts.PhoneNumber).First();
                                ViewBag.ExistingPhone = "Phone already taken";
                                ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", contacts.Company_Id);
                                return View(contacts);
                            }
                            catch
                            {
                                contacts.Updated = DateTime.Now;
                                db.Entry(contacts).State = EntityState.Modified;
                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                        }
                        contacts.Updated = DateTime.Now;
                        db.Entry(contacts).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                if (originalData.PhoneNumber != contacts.PhoneNumber)
                {
                    try
                    {
                        var ExistingPhone = db.Contact.Where(a => a.PhoneNumber != null && a.PhoneNumber == contacts.PhoneNumber).First();
                        ViewBag.ExistingPhone = "Phone already taken";
                        ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", contacts.Company_Id);
                        return View(contacts);
                    }
                    catch
                    {
                        contacts.Updated = DateTime.Now;
                        db.Entry(contacts).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }

                contacts.Updated = DateTime.Now;
                db.Entry(contacts).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Company_Id = new SelectList(db.Company, "Id", "Name", contacts.Company_Id);
            return View(contacts);
        }

        // GET: Contacts/Delete
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Contacts contacts = db.Contact.Find(id);
            contacts.Company = db.Company.Where(a => a.Id == contacts.Company_Id).First();

            if (contacts == null)
            {
                return HttpNotFound();
            }
            return View(contacts);
        }

        // POST: Contacts/Delete
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contacts contacts = db.Contact.Find(id);
            db.Contact.Remove(contacts);
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

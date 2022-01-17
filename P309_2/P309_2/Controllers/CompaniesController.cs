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
    public class CompaniesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public String email;
        public String name;
        public String address;
        public String location;
        public String zipCode;
        public String country;
        public String phone;

        // GET: Companies
        [Authorize]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.NIFSortParm = sortOrder == "NIF" ? "NIF_desc" : "NIF";
            ViewBag.ObservationsSortParm = sortOrder == "Observations" ? "Observations_desc" : "Observations";
            ViewBag.CreatedSortParm = sortOrder == "Created" ? "Created_desc" : "Created";
            ViewBag.UpdatedSortParm = sortOrder == "Updated" ? "Updated_desc" : "Updated";
            ViewBag.PaymentMethodSortParm = sortOrder == "PaymentMethod" ? "PaymentMethod_desc" : "PaymentMethod";
            ViewBag.PaymentDaySortParm = sortOrder == "PaymentDay" ? "PaymentDay_desc" : "PaymentDay";
            ViewBag.UserSortParm = sortOrder == "User" ? "User_desc" : "User";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var companies = from s in db.Company 
                           select s;

            companies = db.Company.Include(c => c.Payment_Method);

            if (!String.IsNullOrEmpty(searchString))
            {
                companies = companies.Where(s => s.Name.Contains(searchString));
            }

            ViewBag.CurrentFilter = searchString;

            switch (sortOrder)
            {
                case "Name_desc":
                    companies = companies.OrderByDescending(s => s.Name);
                    break;
                case "NIF":
                    companies = companies.OrderBy(s => s.NIF);
                    break;
                case "NIF_desc":
                    companies = companies.OrderByDescending(s => s.NIF);
                    break;
                case "Observations":
                    companies = companies.OrderBy(s => s.Observations);
                    break;
                case "Observations_desc":
                    companies = companies.OrderByDescending(s => s.Observations);
                    break;
                case "Created":
                    companies = companies.OrderBy(s => s.Created);
                    break;
                case "Created_desc":
                    companies = companies.OrderByDescending(s => s.Created);
                    break;
                case "Updated":
                    companies = companies.OrderBy(s => s.Updated);
                    break;
                case "Updated_desc":
                    companies = companies.OrderByDescending(s => s.Updated);
                    break;
                case "PaymentMethod":
                    companies = companies.OrderBy(s => s.Payment_Method.Payment_Method);
                    break;
                case "PaymentMethod_desc":
                    companies = companies.OrderByDescending(s => s.Payment_Method.Payment_Method);
                    break;
                case "PaymentDay":
                    companies = companies.OrderBy(s => s.Payment_Day);
                    break;
                case "PaymentDay_desc":
                    companies = companies.OrderByDescending(s => s.Payment_Day);
                    break;
                case "User":
                    companies = companies.OrderBy(s => s.UserId);
                    break;
                case "User_desc":
                    companies = companies.OrderByDescending(s => s.UserId);
                    break;
                default: 
                    companies = companies.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 7; 
            int pageNumber = (page ?? 1); 
            return View(companies.ToPagedList(pageNumber, pageSize)); 
        }

        // GET: Companies/Details/
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Companies companies = db.Company.Find(id);
            companies.Payment_Method = db.Payment_Method.Where(a => a.Id == companies.Payment_Method_Id).First();
            companies.Contacts = db.Contact.Where(a => a.Company_Id == companies.Id).ToList(); 

            if (companies == null)
            {
                return HttpNotFound();
            }
            return View(companies);
        }

        // GET: Companies/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method");
            return View();
        }

        // POST: Companies/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,NIF,Name,Payment_Method_Id,Payment_Day,Created,Updated,UserId,Observations")] Companies companies,FormCollection form)
        {
            ViewBag.ExistingName = null;
            ViewBag.ExistingNIF = null;
            ViewBag.ExistingEmail = null;
            ViewBag.ExistingPhone = null;

            if (ModelState.IsValid)
            {

                email = form["Email"].ToString();
                if (email == "") email = null;
                phone = form["Phone"].ToString();
                if (phone == "") phone = null;
                address = form["Address"].ToString();
                location = form["Location"].ToString();
                zipCode = form["Zip-Code"].ToString();
                country = form["Country"].ToString();
                name = companies.Name + ", Geral";

                try 
                {
                    var ExistingNIF = db.Company.Where(a => a.NIF == companies.NIF).First(); 
                    ViewBag.ExistingNIF = "NIF already taken"; 
                    ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method", companies.Payment_Method_Id);
                    return View(companies);
                }
                catch(Exception)
                {
                    try 
                    {
                        var ExistingName = db.Company.Where(a => a.Name == companies.Name).First(); 
                        ViewBag.ExistingName = "Name already taken"; 
                        ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method", companies.Payment_Method_Id);
                        return View(companies);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            var ExistingEmail = db.Contact.Where(a => a.Email != null && a.Email == email).First();
                            ViewBag.ExistingEmail = "Email already taken";
                            ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method", companies.Payment_Method_Id);
                            return View(companies);
                        }
                        catch (Exception)
                        {
                            try
                            {
                                var ExistingPhone = db.Contact.Where(a => a.PhoneNumber != null && a.PhoneNumber == phone).First();
                                ViewBag.ExistingPhone = "Phone already taken";
                                ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method", companies.Payment_Method_Id);
                                return View(companies);
                            }
                            catch (Exception)
                            {
                                companies.Created = DateTime.Now;
                                companies.Updated = DateTime.Now;
                                companies.UserId = User.Identity.Name;

                                Contacts contact = new Contacts
                                {
                                    Name = name,
                                    Email = email,
                                    PhoneNumber = phone,
                                    Address = address,
                                    ZIP_Code = zipCode,
                                    Country = country,
                                    Location = location,
                                    Company_Id = companies.Id,
                                    Created = companies.Created,
                                    Updated = companies.Updated,
                                    UserId = companies.UserId
                                };

                                db.Company.Add(companies);
                                db.Contact.Add(contact);
                                db.SaveChanges();
                                return RedirectToAction("Index");
                            }
                        }
                    }
                }
            }

            ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method", companies.Payment_Method_Id);
            return View(companies);
        }

        public ActionResult Payment_Methods()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Payment_Methods");
            }
            return View("Error");
        }

        // GET: Companies/Edit
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Companies companies = db.Company.Find(id);

            if (companies == null)
            {
                return HttpNotFound();
            }

            ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method", companies.Payment_Method_Id);
            return View(companies);
        }

        // POST: Companies/Edit
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,NIF,Name,Payment_Method_Id,Payment_Day,Created,Updated,UserId,Observations")] Companies companies)
        {
            var originalData = db.Company.AsNoTracking().Where(s => s.Id == companies.Id).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (originalData.Name != companies.Name)
                {
                    try
                    {
                        var ExistingName = db.Company.Where(a => a.Name == companies.Name).First();
                        ViewBag.ExistingName = "Name already taken";
                        ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method", companies.Payment_Method_Id);
                        return View(companies);
                    }
                    catch
                    {
                        if (originalData.NIF != companies.NIF)
                        {
                            try
                            {
                                var ExistingNIF = db.Company.Where(a => a.NIF == companies.NIF).First();
                                ViewBag.ExistingNIF = "NIF already taken";
                                ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method", companies.Payment_Method_Id);
                                return View(companies);
                            }
                            catch
                            {
                                try
                                {
                                    Contacts contact = db.Contact.Where(a => a.Company_Id == companies.Id).First();
                                    contact.Name = companies.Name + ", Geral";
                                    companies.Updated = DateTime.Now;
                                    db.Entry(companies).State = EntityState.Modified;
                                    db.Entry(contact).State = EntityState.Modified;
                                    db.SaveChanges();
                                    return RedirectToAction("Index");
                                }
                                catch
                                {
                                    ViewBag.NoContact = "Create, at least, one contact related to selected company";
                                    ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method", companies.Payment_Method_Id);
                                    return View(companies);
                                }
                                
                            }
                        }

                        try
                        {
                            Contacts contact = db.Contact.Where(a => a.Company_Id == companies.Id).First();
                            contact.Name = companies.Name + ", Geral";
                            companies.Updated = DateTime.Now;
                            db.Entry(companies).State = EntityState.Modified;
                            db.Entry(contact).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        catch
                        {
                            ViewBag.NoContact = "Create, at least, one contact related to selected company";
                            ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method", companies.Payment_Method_Id);
                            return View(companies);
                        }
                    }
                }

                if (originalData.NIF != companies.NIF)
                {
                    try
                    {
                        var ExistingNIF = db.Company.Where(a => a.NIF == companies.NIF).First();
                        ViewBag.ExistingNIF = "NIF already taken";
                        ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method", companies.Payment_Method_Id);
                        return View(companies);
                    }
                    catch
                    {
                        companies.Updated = DateTime.Now;
                        db.Entry(companies).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }

                companies.Updated = DateTime.Now;
                db.Entry(companies).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Payment_Method_Id = new SelectList(db.Payment_Method, "Id", "Payment_Method", companies.Payment_Method_Id);
            return View(companies);
        }

        // GET: Companies/Delete
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Companies companies = db.Company.Find(id);
            companies.Payment_Method = db.Payment_Method.Where(a => a.Id == companies.Payment_Method_Id).First();

            if (companies == null)
            {
                return HttpNotFound();
            }
            return View(companies);
        }

        // POST: Companies/Delete
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Companies companies = db.Company.Find(id);
            db.Company.Remove(companies);
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

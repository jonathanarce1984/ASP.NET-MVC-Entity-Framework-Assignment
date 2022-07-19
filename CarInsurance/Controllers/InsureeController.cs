/* Jonathan Arce
 * ASP.NET MVC Entity Framework Assignment
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Firstname,Lastname,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Qoute")] Insuree insuree, string Create, string Calculate)
        {
            if (Create != null)
            {
                if (ModelState.IsValid)
                {
                    db.Insurees.Add(insuree);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            if (Calculate != null) {
                ModelState.Remove("Qoute");
                double V = calculateQuote(insuree);
                insuree.Qoute = (decimal)V;
            }

                return View(insuree);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Firstname,Lastname,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Qoute")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
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
        public double calculateQuote(Insuree insuree)
        {
            if (insuree == null)
            {
                return 0;
            }
            double total = 50; // Start with a base of $50 / month.

            int now = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
            int dob = int.Parse(insuree.DateOfBirth.ToString("yyyyMMdd"));
            int age = (now - dob) / 10000;

            //If the user is 18 and under, add $100 to the monthly total
            if (age <= 18)
            {
                total += 100;
            }
            //If the user is between 19 and 25, add $50 to the monthly total.
            else if (age <= 19 && age >= 25) {
                total += 50;
            }
            //If the user is over 25, add $25 to the monthly total.
            else if (age > 25) {
                total += 25;
            }
            //If the car's year is before 2000, add $25 to the monthly total.
            if (insuree.CarYear < 2000) {
                total += 25;
            }
            //If the car's year is after 2015, add $25 to the monthly total.
            else if (insuree.CarYear > 2015)
            {
                total += 25;
            }

            //If the car's Make is a Porsche, add $25 to the price.
            if (insuree.CarMake.ToLower() == "porsche")
            {
                total += 25;
            }
            //If the car's Make is a Porsche and its model is a 911 Carrera, add an additional $25 to the price.
            else if (insuree.CarMake.ToLower() == "porsche" && insuree.CarModel.ToLower() == "911 carrera")
            {
                total += 25;
            }
            //Add $10 to the monthly total for every speeding ticket the user has.
            total += insuree.SpeedingTickets * 10;

            //If the user has ever had a DUI, add 25% to the total.
            if (insuree.DUI) {
                total = total * (1 + 0.25);
            }
            //If it's full coverage, add 50% to the total.
            if (insuree.CoverageType)
            {
                total = total * (1 + 0.50);
            }
            return total;
        }

        //Create an Admin view for a site administrator. This page must Show all quotes issued, along with the user's first name, last name, and email address.
        public ActionResult Admin()
        {
            ViewBag.Message = "Admin View Page";
            return View(db.Insurees.ToList());
        }

    }

    
}

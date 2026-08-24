using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ShopMate.Controllers
{
    public class InvoicePaymentMethodController : Controller
    {
        string userId = Env.GetUserInfo("name");
        private SIContext db = new SIContext();
        // GET: InvoicePaymentMethod
        public ActionResult Index()
        {
            //return View();
            return View(db.InvoicePaymentMethods.ToList());
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoicePaymentMethod invPaymentMethod = db.InvoicePaymentMethods.Find(id);
            if (invPaymentMethod == null)
            {
                return HttpNotFound();
            }
            return View(invPaymentMethod);
        }
        public ActionResult GetGrid()
        {
            var tak = db.InvoicePaymentMethods.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Name),
            Convert.ToString(c.DueIn),
            //Convert.ToString(c.IsPurchaseOrSale),
            //Convert.ToString(c.AddedBy),
            //Convert.ToString(c.DateAdded),
            //Convert.ToString(c.DateModied),
            //Convert.ToString(c.ModifiedBy),
            // Convert.ToString(c.WarehouseId),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }

        // GET: InvoicePaymentMethods/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InvoicePaymentMethods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,DueIn")] InvoicePaymentMethod invPaymentMethod)
        {
            if (ModelState.IsValid)
            {
                db.InvoicePaymentMethods.Add(invPaymentMethod);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(invPaymentMethod);
        }

        // GET: InvoicePaymentMethods/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoicePaymentMethod invPaymentMethod = db.InvoicePaymentMethods.Find(id);
            if (invPaymentMethod == null)
            {
                return HttpNotFound();
            }
            return View(invPaymentMethod);
        }

        // POST: InvoicePaymentMethods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,DueIn")] InvoicePaymentMethod invPaymentMethod)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invPaymentMethod).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(invPaymentMethod);
        }

        // GET: InvoicePaymentMethods/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoicePaymentMethod InvPaymentMethod = db.InvoicePaymentMethods.Find(id);
            if (InvPaymentMethod == null)
            {
                return HttpNotFound();
            }
            return View(InvPaymentMethod);
        }

        // POST: InvoicePaymentMethods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvoicePaymentMethod InvPaymentMethod = db.InvoicePaymentMethods.Find(id);
            db.InvoicePaymentMethods.Remove(InvPaymentMethod);
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
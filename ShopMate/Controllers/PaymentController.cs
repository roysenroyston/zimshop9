using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class PaymentController : Controller
    {
        string userId = Env.GetUserInfo("name");
        private SIContext db = new SIContext();

        // GET: Payments
        public ActionResult Index()
        {
            return View(db.Payments.ToList());
        }

        public ActionResult GetGrid()
        {
            var tak = db.Payments.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(db.Users.FirstOrDefault(i => i.Id == c.CustomerId).UserName),
            //Convert.ToString(c.InvoiceId),
             Convert.ToString(c.PaymentDate),
            Convert.ToString(c.Amount),
            Convert.ToString(db.PaymentModes.FirstOrDefault(i => i.Id == c.PaymentModeId).Name),
            Convert.ToString(c.CurrencyAmount),
            //Convert.ToString(db.Banks.FirstOrDefault(i => i.Id == c.BankId).Name),
            //Convert.ToString(c.BankAccount),
           
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: Payments/Create
        public ActionResult Create()
        {
            ViewBag.CustomerId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Customer" ||  i.Role_RoleId.RoleName == "Supplier"), "Id", "UserName");
            ViewBag.PaymentModeId = new SelectList(db.PaymentModes, "Id", "Name");
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Name");
            ViewBag.CurrencyId = new SelectList(db.Currencies, "Id", "Name");
            ViewBag.PaymentTypeId = new SelectList(db.PaymentTypes, "Id", "Name");            
            return View();
        }

       
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(int customerid, int PaymentModeId, int? BankId, string BankAccount, int? InvoiceId, decimal? Amount, string PaymentReference, decimal CurrencyAmount)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string returval = "Succeful";
            try {            
            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            if (ModelState.IsValid)
            {
                Payment payment = new Payment();
                payment.CustomerId = customerid;
                //payment.PaymentTypeId = db.PaymentTypes.FirstOrDefault(i => i.Name == "Credit").Id;
                payment.PaymentModeId = PaymentModeId;
                payment.BankId = BankId;
                payment.BankAccount = BankAccount;
                //payment.InvoiceId = InvoiceId;
                payment.Amount = Amount;
                payment.PaymentReference = PaymentReference;
                payment.AddedBy = AddedBy;
                payment.PaymentDate = DateTime.Now;
                payment.CurrencyAmount = CurrencyAmount;
                db.Payments.Add(payment);
                db.SaveChanges();
                //return RedirectToAction("Index");
            }

                    var selectedInvoices = db.Invoices.Where(i => i.CustomerId == customerid && i.balance > 0);
                    //db.Invoices.Where(i => i.CustomerId == (id) && i.balance > 0).ToArray();
                    if (selectedInvoices != null)
                    {
                        do
                        {
                            foreach (var item in selectedInvoices)
                            {
                            decimal? money = 0;
                            if (Amount > 0)
                                {
                                   
                                    money = item.balance;
                                    item.IsBilled = true;
                                    item.payment = money;
                                    item.balance = item.balance - money;
                                    
                                    db.Entry(item).State = EntityState.Modified;
                                        Amount -= money;
                                   
                                }
                               
                                //db.SaveChanges();
                            }
                           
                            db.SaveChanges(userId);
                        }
                        while (Amount > 0);


                    }



               
                //}


                sb.Append("Submitted");
            returval = "Submitted";
            return Json(returval, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }
            return Content(sb.ToString());
        }
       
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,AccountNumber")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(payment);
        }

        // GET: Payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payment payment = db.Payments.Find(id);
            db.Payments.Remove(payment);
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
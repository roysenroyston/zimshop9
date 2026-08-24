using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ShopMate.Models;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class DuePaymentController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        // GET: /DuePayment/
        public ActionResult Index()
        {
            return View();
        }

        // GET DuePayment/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.DuePayments.ToArray();
try { 
            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id), 
            Convert.ToString(c.User_UserId.UserName), 
            Convert.ToString(c.DueAmount), 
            Convert.ToString(c.Remarks), 
            Convert.ToString(c.AddedBy), 
            Convert.ToString(c.DateAdded), 
            Convert.ToString(c.WarehouseId), 
             Convert.ToString(c.IsReturn), 
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);}
            catch (InvalidOperationException ex)
            {
                Helper.WriteError(ex, ex.Message);

                return View(ex.Message);


    }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View("System encountered an unknown error.");
}
        }
        // GET: /DuePayment/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: /DuePayment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DuePayment ObjDuePayment = db.DuePayments.Find(id);
            if (ObjDuePayment == null)
            {
                return HttpNotFound();
            }
            return View(ObjDuePayment);
        }
        // GET: /DuePayment/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users.Where(i => i.RoleId == 4 || i.RoleId == 3), "Id", "UserName");

            return View();
        }

        // POST: /DuePayment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(DuePayment ObjDuePayment)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {

                    int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
                    ObjDuePayment.IsReturn = false;
                    db.DuePayments.Add(ObjDuePayment);
                    db.SaveChanges(userId);


                    //transaction
                    Transaction tr = new Transaction();
                    tr.AddedBy = ObjDuePayment.AddedBy;
                    tr.DebitLedgerAccountId = db.LedgerAccounts.FirstOrDefault( i => i.Name == "Money Invested").Id;
                    tr.DebitAmount = ObjDuePayment.DueAmount;
                    tr.CreditLedgerAccountId = db.LedgerAccounts.FirstOrDefault(i => i.Name == "Money Invested").Id; ;
                    tr.CreditAmount = ObjDuePayment.DueAmount;
                    tr.DateAdded = DateTime.Now;
                    tr.Remarks = "DuePayment, Cash Account debit and User account credit";
                    tr.Other = null;
                    tr.PurchaseOrSale = "DuePayment";
                    tr.PurchaseIdOrSaleId = ObjDuePayment.Id;
                    tr.WarehouseId = warehouse;

                    db.Transactions.Add(tr);
                    db.SaveChanges(userId);

                    sb.Append("Sumitted");
                    return Content(sb.ToString());
                }
                else
                {
                    foreach (var key in this.ViewData.ModelState.Keys)
                    {
                        foreach (var err in this.ViewData.ModelState[key].Errors)
                        {
                            sb.Append(err.ErrorMessage + "<br/>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());

        }


        public ActionResult Returned(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DuePayment ObjDuePayment = db.DuePayments.Find(id);
            if (ObjDuePayment.IsReturn == false)
            {
                ObjDuePayment.IsReturn = true;
                db.Entry(ObjDuePayment).State = EntityState.Modified;

                Transaction ObjTransaction = db.Transactions.FirstOrDefault(i => i.PurchaseOrSale == "DuePayment" && i.PurchaseIdOrSaleId == id);

                int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));

                //transaction
                Transaction tr = new Transaction();
                tr.AddedBy = ObjDuePayment.AddedBy;
                tr.DebitLedgerAccountId = ObjTransaction.CreditLedgerAccountId;
                tr.DebitAmount = ObjDuePayment.DueAmount;
                tr.CreditLedgerAccountId = ObjTransaction.DebitLedgerAccountId;
                tr.CreditAmount = ObjDuePayment.DueAmount;
                tr.DateAdded = DateTime.Now;
                tr.Remarks = "DuePayment, User Account debit and cash account credit";
                tr.Other = null;
                tr.PurchaseOrSale = "DuePaymentReturn";
                tr.PurchaseIdOrSaleId = ObjDuePayment.Id;
                tr.WarehouseId = warehouse;

                db.Transactions.Add(tr);
                db.SaveChanges(userId);
            }

            if (ObjDuePayment == null)
            {
                return HttpNotFound();
            }

            return View();
        }


        // GET: /DuePayment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DuePayment ObjDuePayment = db.DuePayments.Find(id);
            if (ObjDuePayment == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", ObjDuePayment.UserId);

            return View(ObjDuePayment);
        }

        // POST: /DuePayment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(DuePayment ObjDuePayment)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjDuePayment).State = EntityState.Modified;
                    db.SaveChanges(userId);

                    sb.Append("Sumitted");
                    return Content(sb.ToString());
                }
                else
                {
                    foreach (var key in this.ViewData.ModelState.Keys)
                    {
                        foreach (var err in this.ViewData.ModelState[key].Errors)
                        {
                            sb.Append(err.ErrorMessage + "<br/>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }


            return Content(sb.ToString());

        }
        // GET: /DuePayment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DuePayment ObjDuePayment = db.DuePayments.Find(id);
            if (ObjDuePayment == null)
            {
                return HttpNotFound();
            }
            return View(ObjDuePayment);
        }

        // POST: /DuePayment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                DuePayment ObjDuePayment = db.DuePayments.Find(id);
                try
                { 
                    Transaction ObjTransaction = db.Transactions.FirstOrDefault(i => i.PurchaseOrSale == "DuePayment" && i.PurchaseIdOrSaleId == id);
                    db.Transactions.Remove(ObjTransaction);
                }
                catch (Exception ex)
                {
                    Helper.WriteError(ex, ex.Message);
                }
                db.DuePayments.Remove(ObjDuePayment);
                db.SaveChanges(userId);

                sb.Append("Sumitted");
                return Content(sb.ToString());

            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());

        }
        // GET: /DuePayment/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            DuePayment ObjDuePayment = db.DuePayments.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                ViewBag.UserId = new SelectList(db.Users, "Id", "UserName", ObjDuePayment.UserId);

            }

            return View(ObjDuePayment);
        }

        // POST: /DuePayment/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(DuePayment ObjDuePayment)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjDuePayment).State = EntityState.Modified;
                    db.SaveChanges(userId);

                    sb.Append("Sumitted");
                    return Content(sb.ToString());
                }
                else
                {
                    foreach (var key in this.ViewData.ModelState.Keys)
                    {
                        foreach (var err in this.ViewData.ModelState[key].Errors)
                        {
                            sb.Append(err.ErrorMessage + "<br/>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());

        }

        private SIContext db = new SIContext();


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


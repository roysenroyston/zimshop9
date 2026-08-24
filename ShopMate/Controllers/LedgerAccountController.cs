using ShopMate.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class LedgerAccountController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        int WareId = int.Parse(Env.GetUserInfo("WarehouseId"));
        // GET: /LedgerAccount/
        public ActionResult Index()
        {
            return View();
        }

        // GET LedgerAccount/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.LedgerAccounts.Where(l => l.WarehouseId == WareId).ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Name),
            Convert.ToString(c.ParentId),
Convert.ToString(c.DateAdded),
            Convert.ToString(c.AddedBy),
            Convert.ToString(c.WarehouseId),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: /LedgerAccount/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: /LedgerAccount/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LedgerAccount ObjLedgerAccount = db.LedgerAccounts.Find(id);
            if (ObjLedgerAccount == null)
            {
                return HttpNotFound();
            }
            return View(ObjLedgerAccount);
        }
        // GET: /LedgerAccount/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.LedgerAccounts.Where(n => n.ParentId == null && n.Name == "EXPENSE"), "Id", "Name");

            return View();
        }

        // POST: /LedgerAccount/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(LedgerAccount ObjLedgerAccount)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.LedgerAccounts.Add(ObjLedgerAccount);
                    db.SaveChanges();

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
        // GET: /LedgerAccount/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LedgerAccount ObjLedgerAccount = db.LedgerAccounts.Find(id);
            if (ObjLedgerAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.LedgerAccounts, "Id", "Name", ObjLedgerAccount.ParentId);

            return View(ObjLedgerAccount);
        }

        // POST: /LedgerAccount/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(LedgerAccount ObjLedgerAccount)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjLedgerAccount).State = EntityState.Modified;
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
        // GET: /LedgerAccount/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LedgerAccount ObjLedgerAccount = db.LedgerAccounts.Find(id);
            if (ObjLedgerAccount == null)
            {
                return HttpNotFound();
            }
            return View(ObjLedgerAccount);
        }

        // POST: /LedgerAccount/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                LedgerAccount ObjLedgerAccount = db.LedgerAccounts.Find(id);
                db.LedgerAccounts.Remove(ObjLedgerAccount);
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
        // GET: /LedgerAccount/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            LedgerAccount ObjLedgerAccount = db.LedgerAccounts.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                ViewBag.ParentId = new SelectList(db.LedgerAccounts, "Id", "Name", ObjLedgerAccount.ParentId);

            }

            return View(ObjLedgerAccount);
        }

        // POST: /LedgerAccount/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(LedgerAccount ObjLedgerAccount)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjLedgerAccount).State = EntityState.Modified;
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
        public ActionResult TransactionGetGrid(int id = 0)
        {
            var tak = db.Transactions.Where(i => i.CreditLedgerAccountId == id).ToArray();

            var result = from c in tak
                         select new string[] { Convert.ToString(c.Id),Convert.ToString(c.Id),
                Convert.ToString(c.DebitLedgerAccountId),
                Convert.ToString(c.DebitAmount),
                Convert.ToString(c.CreditLedgerAccountId),
                Convert.ToString(c.CreditAmount),
                Convert.ToString(c.AddedBy),
                Convert.ToString(c.DateAdded),
                Convert.ToString(c.Other),
                Convert.ToString(c.PurchaseOrSale),
                Convert.ToString(c.PurchaseIdOrSaleId),
                 };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TransactionGetGrid1(int id = 0)
        {
            var tak = db.Transactions.Where(i => i.CreditLedgerAccountId == id).ToArray();

            var result = from c in tak
                         select new string[] { Convert.ToString(c.Id),Convert.ToString(c.Id),
                Convert.ToString(c.DebitLedgerAccountId),
                Convert.ToString(c.DebitAmount),
                Convert.ToString(c.CreditLedgerAccountId),
                Convert.ToString(c.CreditAmount),
                Convert.ToString(c.AddedBy),
                Convert.ToString(c.DateAdded),
                Convert.ToString(c.Other),
                Convert.ToString(c.PurchaseOrSale),
                Convert.ToString(c.PurchaseIdOrSaleId),
                 };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
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


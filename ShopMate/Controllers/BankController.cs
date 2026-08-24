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
    public class BankController : Controller
    {
        string userId = Env.GetUserInfo("name");
        private SIContext db = new SIContext();

        public ActionResult GetGrid()
        {
            var tak = db.Banks.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Name),
            Convert.ToString(c.AccountNumber),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: Banks
        public ActionResult Index()
        {
            return View(db.Banks.ToList());
        }

        // GET: Banks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bank bank = db.Banks.Find(id);
            if (bank == null)
            {
                return HttpNotFound();
            }
            return View(bank);
        }

        // GET: Banks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Banks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,AccountNumber")] Bank bank)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Banks.Add(bank);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(bank);
        //}
        public ActionResult Create(Bank ObjBank)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Bank not added! The bank already exists.";
            var tak = db.Banks.Where(i => i.Name == ObjBank.Name).FirstOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (tak != null)
                    {
                        sb.Append("Bank Already Exist");
                        result = "Bank Already Exist";
                        //  return Content(sb.ToString());
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db.Banks.Add(ObjBank);
                        db.SaveChanges(userId);

                        sb.Append("Submitted");
                        result = "Submitted";

                        return Json(result, JsonRequestBehavior.AllowGet);
                        //return View(result);

                    }


                    // sb.Append("Sumitted");
                    //  return Content(sb.ToString());
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

        // GET: Banks/Edit/5
        public ActionResult Edit(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Bank bank = db.Banks.Find(id);
            //if (bank == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(bank);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bank Objbank = db.Banks.Find(id);
            if (Objbank == null)
            {
                return HttpNotFound();
            }
            //ViewBag.TaxId = new SelectList(db.Taxs, "Id", "Name", Objbank.TaxId);

            return View(Objbank);
        }

        // POST: Banks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Name,AccountNumber")] Bank bank)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(bank).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(bank);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Bank ObjBank)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            try
            {
                //  List<ProductStock> stock = db.ProductStocks.Where(i=>i.Id==ObjProduct.ProductStock_ProductIds);
                if (ModelState.IsValid)
                {


                    //ObjProduct.RemainingQuantity = ObjProduct.RemainingQuantity;
                    db.Entry(ObjBank).State = EntityState.Modified;
                    // db.SaveChanges();
                    //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
                    // var selectedProduct = db.Products.FirstOrDefault(i => i.Id == ObjPurchase.ProductId);
                    //var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == ObjProduct.TaxId);

                    sb.Append("Sumitted");



                    db.SaveChanges(userId);


                    //end
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

        // GET: Banks/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Bank bank = db.Banks.Find(id);
        //    if (bank == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bank);
        //}

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bank ObjBank = db.Banks.Find(id);
            if (ObjBank == null)
            {
                return HttpNotFound();
            }
            return View(ObjBank);
        }

        // POST: Banks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Bank bank = db.Banks.Find(id);
        //    db.Banks.Remove(bank);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                Bank ObjBank = db.Banks.Find(id);
                db.Banks.Remove(ObjBank);
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
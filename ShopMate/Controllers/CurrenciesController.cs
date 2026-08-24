using ShopMate.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class CurrenciesController : Controller
    {
        string userId = Env.GetUserInfo("name");
        int wareId = int.Parse(Env.GetUserInfo("WarehouseId"));
        private SIContext db = new SIContext();

        public ActionResult GetGrid()
        {

            var tak = db.Currencies.Where(k => k.WarehouseId == wareId).ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Name),
              Convert.ToString(c.CurrencySymbol),
                   Convert.ToString(db.Warehouses.FirstOrDefault(k=> k.Id ==c.WarehouseId).Name),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: Currencies
        public ActionResult Index()
        {
            return View(db.Currencies.ToList());
        }

        // GET: Currencies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = db.Currencies.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // GET: Currencies/Create
        public ActionResult Create()
        {
            ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(k => k.Id == wareId), "Id", "Name");
            return View();
        }

        // POST: Currencies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name")] Currency currency)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Currencies.Add(currency);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(currency);
        //}
        public ActionResult Create(Currency ObjCurrency)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Currency not added! The currency already exist.";
            var tak = db.Currencies.Where(i => i.Name == ObjCurrency.Name && i.WarehouseId == ObjCurrency.WarehouseId).FirstOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (tak != null)
                    {
                        sb.Append("Currency Already Exists ");
                        result = "Currency Already Exists";
                        //  return Content(sb.ToString());
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db.Currencies.Add(ObjCurrency);
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

        // GET: Currencies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = db.Currencies.Find(id);
            ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // POST: Currencies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Name")] Currency currency)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(currency).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(currency);
        //}
        public ActionResult Edit(Currency ObjCurrency)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            try
            {
                //  List<ProductStock> stock = db.ProductStocks.Where(i=>i.Id==ObjProduct.ProductStock_ProductIds);
                if (ModelState.IsValid)
                {


                    //ObjProduct.RemainingQuantity = ObjProduct.RemainingQuantity;
                    db.Entry(ObjCurrency).State = EntityState.Modified;
                    // db.SaveChanges();
                    //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
                    // var selectedProduct = db.Products.FirstOrDefault(i => i.Id == ObjPurchase.ProductId);
                    //var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == ObjCurrency.TaxId);

                    sb.Append("Submitted");



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

        // GET: Currencies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = db.Currencies.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // POST: Currencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Currency currency = db.Currencies.Find(id);
        //    db.Currencies.Remove(currency);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                Currency ObjCurrency = db.Currencies.Find(id);
                db.Currencies.Remove(ObjCurrency);
                db.SaveChanges(userId);

                sb.Append("Submitted");
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

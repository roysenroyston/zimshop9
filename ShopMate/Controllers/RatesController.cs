using ShopMate.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ShopMate.Controllers
{
    public class RatesController : Controller
    {
        string userId = Env.GetUserInfo("name");
        // GET: Rates
        public ActionResult Index()
        {
            return View();
        }
        //get rare
        private SIContext db = new SIContext();
        public ActionResult GetGrid()
        {
            var WareId = db.Users.FirstOrDefault(n => n.UserName == userId).WarehouseId;
            var tak = db.Rates.ToArray();
            var Mode = db.PaymentModes.ToArray();

            if (WareId == 1)
            {

                var result = from c in tak
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
                         Convert.ToString(db.Currencies.FirstOrDefault(i => i.Id == (c.CurrencyId)).Name),
                         Convert.ToString(c.CurrencyRate),
                         Convert.ToString(c.DateModified),
                        Convert.ToString(db.Warehouses.FirstOrDefault(m=> m.Id == c.WarehouseId).Name)

             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var result = from c in tak.Where(i => i.WarehouseId == WareId)
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
                         Convert.ToString(db.Currencies.FirstOrDefault(i => i.Id == (c.CurrencyId)).Name),
                         Convert.ToString(c.CurrencyRate),
                         Convert.ToString(c.DateModified),
                            Convert.ToString(db.Warehouses.FirstOrDefault(m=> m.Id == c.WarehouseId).Name)

             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }

        }
        // GET: Rates/Details/5
        public ActionResult Details(int id)
        {

            return View();
        }

        // GET: Rates/Create
        public ActionResult Create()
        {

            //var paye = db.PaymentModes.Where(i => i.Id == 1);  
            var userwarehouse = db.Users.FirstOrDefault(n => n.UserName == userId).WarehouseId;




            ViewBag.Currency = new SelectList(db.Currencies.Where(n => n.WarehouseId == userwarehouse), "Id", "Name");



            //        ViewBag.PaymentModeId = db.PaymentModes
            // .Select(e => new SelectListItem
            // {
            //     Value = e.Id.ToString(),
            //     Text = e.Name
            // })
            //.ToList();
            return View();
        }

        // POST: Rates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Rate rate)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                var WareId = db.Users.FirstOrDefault(n => n.UserName == userId).WarehouseId;
                var userwarehouse = db.Users.FirstOrDefault(n => n.UserName == userId).WarehouseId;

                ViewBag.Currency = new SelectList(db.Currencies.Where(n => n.WarehouseId == userwarehouse), "Id", "Name");

                if (ModelState.IsValid)
                {
                    rate.WarehouseId = (int)WareId;
                    db.Rates.Add(rate);
                    db.SaveChanges();

                    sb.Append("Sumitted");
                    //return Content(sb.ToString());
                    return View();
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
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());

        }
        // GET: Rates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rate ObjRole = db.Rates.Find(id);
            if (ObjRole == null)
            {
                return HttpNotFound();
            }

            return View(ObjRole);
        }

        // POST: /Role/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Rate ObjRole)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjRole).State = EntityState.Modified;
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
                sb.Append("Error :" + ex.Message);
            }


            return Content(sb.ToString());

        }
        // GET: Rates/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Rates/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Rate(int PaymentModeId, decimal GrossAmount)
        {
            var rate = db.Rates.FirstOrDefault(i => i.CurrencyId == (PaymentModeId)).CurrencyRate;

            double RateAmount = (double)GrossAmount * rate;

            ViewBag.RateAmount = RateAmount;
            return View();
        }
    }
}

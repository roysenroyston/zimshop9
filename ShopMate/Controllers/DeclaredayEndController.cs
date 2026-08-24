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
    public class DeclaredayEndController : Controller
    {
        string userId = Env.GetUserInfo("name");
        string warehouseId = Env.GetUserInfo("WarehouseId");
        // GET: DeclaredayEnd
        public ActionResult Index()
        {
            return View();
        }
        private SIContext db = new SIContext();
        public ActionResult GetGrid()
        {
            var tak = db.DayEnds.ToArray();
            var user = db.Users.ToArray();
            var userWarehouse = db.Users.FirstOrDefault(i => i.UserName == userId).WarehouseId;

            if (userId == "Zimhope")
            {
                var result = from c in tak
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),

            Convert.ToString(c.totalcash),
            Convert.ToString(c.totalCashUsd),
            Convert.ToString(c.ecocash),
           // Convert.ToString(c.Fbc),
           // Convert.ToString(c.Acl),
            Convert.ToString(c.Zipit),
            Convert.ToString(c.totalAmount),
             Convert.ToString(db.Users.FirstOrDefault(m=> m.Id == c.AddedBy).UserName),
              Convert.ToString(db.Users.FirstOrDefault(m=> m.Id == c.ModifiedBy).UserName),
            Convert.ToString(c.DateAdded),
            Convert.ToString(c.Declared),
            Convert.ToString(c.WarehouseId),
             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = from c in tak.Where(n => n.WarehouseId == userWarehouse)
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),

            Convert.ToString(c.totalcash),
            Convert.ToString(c.totalCashUsd),
            Convert.ToString(c.ecocash),
          //  Convert.ToString(c.Fbc),
          //  Convert.ToString(c.Acl),
            Convert.ToString(c.Zipit),

            Convert.ToString(c.totalAmount),

               Convert.ToString(db.Users.FirstOrDefault(m=> m.Id == c.AddedBy).UserName),
              Convert.ToString(db.Users.FirstOrDefault(m=> m.Id == c.ModifiedBy).UserName),

            Convert.ToString(c.DateAdded),
            Convert.ToString(c.Declared),
            Convert.ToString(c.WarehouseId),
             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: DeclaredayEnd/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeclaredayEnd takings = db.DayEnds.Find(id);
            if (takings == null)
            {
                return HttpNotFound();
            }
            return View(takings);
        }
        // GET: DeclaredayEnd/Create
        public ActionResult Create()
        {
            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            var userCustomers = db.Users.FirstOrDefault(n => n.UserName == userId).WarehouseId;

            if (userId == "Zimhope")
            {
                ViewBag.WareHouse = new SelectList(db.Warehouses, "Id", "Name");
                ViewBag.Tilloperator = db.Users.FirstOrDefault(i => i.Id == AddedBy).UserName;
                ViewBag.AddedBy = new SelectList(db.Users.Where(m => m.RoleId == 2 && m.IsActive == true), "Id", "UserName");
            }
            else
            {
                ViewBag.WareHouse = new SelectList(db.Warehouses.Where(n => n.Id == userCustomers), "Id", "Name");
                ViewBag.Tilloperator = db.Users.FirstOrDefault(i => i.Id == AddedBy).UserName;
                ViewBag.AddedBy = new SelectList(db.Users.Where(m => m.RoleId == 2 && m.IsActive == true && m.WarehouseId == userCustomers), "Id", "UserName");
            }


            return View();
        }

        // POST: DeclaredayEnd/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(DeclaredayEnd en)
        {
            //int AddedBy =  Convert.ToInt32(Env.GetUserInfo("WarehouseId"));
            var wareId = Convert.ToInt32(Env.GetUserInfo("WarehouseId"));
            string result = "Error! Product not added! The Product already exist.";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {
                    en.WarehouseId = (int)wareId;
                    en.AddedBy = en.AddedBy;
                    en.ModifiedBy = db.Users.FirstOrDefault(m => m.UserName == userId).Id;
                    en.DateAdded = DateTime.Now;
                    db.DayEnds.Add(en);
                    db.SaveChanges(userId);

                    //sb.Append("Sumitted");
                    //return Content(sb.ToString());
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

            //return Content(sb.ToString());
            result = "Success! Dayend Completed";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: DeclaredayEnd/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DeclaredayEnd ObjInvoiceItems = db.DayEnds.Find(id);
            if (ObjInvoiceItems == null)
            {
                return HttpNotFound();
            }
            // ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ObjInvoiceItems.ProductId);

            return View(ObjInvoiceItems);
        }

        // POST: DeclaredayEnd/Edit/5
        [HttpPost]
        public ActionResult Edit(DeclaredayEnd en)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(en).State = EntityState.Modified;
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

        // GET: DeclaredayEnd/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DeclaredayEnd/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View();
            }
        }
    }
}

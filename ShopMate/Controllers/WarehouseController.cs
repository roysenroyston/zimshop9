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
    public class WarehouseController : BaseController
    {
        // GET: /Warehouse/
        public ActionResult Index()
        {
            return View();
        }

        // GET Warehouse/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.Warehouses.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Name),
            Convert.ToString(c.Address),
            Convert.ToString(c.Mobile),
            Convert.ToString(c.Email),
                    Convert.ToString(c.NumberOfUsers),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: /Warehouse/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: /Warehouse/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse ObjWarehouse = db.Warehouses.Find(id);
            if (ObjWarehouse == null)
            {
                return HttpNotFound();
            }
            return View(ObjWarehouse);
        }
        // GET: /Warehouse/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: /Warehouse/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Warehouse ObjWarehouse)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {

                    ObjWarehouse.DateCreated = DateTime.Now.Date;

                    db.Warehouses.Add(ObjWarehouse);
                    db.SaveChanges();
                    //var products = db.Products.ToList();
                    //foreach (var product in products)
                    //{
                    //    WarehouseStock newWareHouse = new WarehouseStock();
                    //    newWareHouse.ProductId = product.Id;
                    //    newWareHouse.RemainingQuantity = 0;
                    //    newWareHouse.WarehouseId = ObjWarehouse.Id;
                    //    db.WarehouseStocks.Add(newWareHouse);
                    //    db.SaveChanges();
                    //}

                    sb.Append("Submitted");
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
        // GET: /Warehouse/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse ObjWarehouse = db.Warehouses.Find(id);
            if (ObjWarehouse == null)
            {
                return HttpNotFound();
            }

            return View(ObjWarehouse);
        }
        public ActionResult Activate(int? id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse ObjWarehouse = db.Warehouses.Find(id);
            var objUser = db.Users.Where(w => w.WarehouseId == id).ToList();
            if (ObjWarehouse == null)
            {
                return HttpNotFound();
            }

            foreach (var user in objUser)
            {
                DateTime today = (DateTime)user.JoinDate;
                // var duein =  DateTime.Subtract(today, 90);
                TimeSpan ts = new TimeSpan(365, 0, 0, 0);
                DateTime? answer = today.Add(ts);

                var ngonie = db.Users.FirstOrDefault(h => h.Id == user.Id);
                ngonie.JoinDate = answer;
                db.Entry(ngonie).State = EntityState.Modified;
                db.SaveChanges();
            }
            //DateTime dateOfJoining = (DateTime)login.JoinDate; // Example

            //// Calculate time difference
            //TimeSpan timeDifference = DateTime.Now - dateOfJoining;

            //// Check if one year has passed
            //if (timeDifference.TotalDays >= 365)
            //{
            //    ModelState.AddModelError(string.Empty, "You are not allowed to log in as one year has passed since your date of joining.");
            //    //    ViewBag.Msg = "Your Account Expired, Contact 0783 284 440";
            //    return Request.CreateResponse(HttpStatusCode.Forbidden, "Your Account Expired, Contact 0783 284 440");
            //}
            sb.Append("Account Activated Successfully");
            return Content(sb.ToString());
        }
        // POST: /Warehouse/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Warehouse ObjWarehouse)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjWarehouse).State = EntityState.Modified;
                    db.SaveChanges();

                    sb.Append("Submitted");
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

        // GET: /Warehouse/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse ObjWarehouse = db.Warehouses.Find(id);
            if (ObjWarehouse == null)
            {
                return HttpNotFound();
            }
            return View(ObjWarehouse);
        }

        // POST: /Warehouse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                var products = db.WarehouseStocks.Where(i => i.WarehouseId == id);
                foreach (var product in products.ToList())
                {
                    WarehouseStock warehouseStock = db.WarehouseStocks.Find(product.Id);
                    db.WarehouseStocks.Remove(warehouseStock);
                    db.SaveChanges();
                }

                Warehouse ObjWarehouse = db.Warehouses.Find(id);
                db.Warehouses.Remove(ObjWarehouse);
                db.SaveChanges();

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
        // GET: /Warehouse/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            Warehouse ObjWarehouse = db.Warehouses.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;

            }

            return View(ObjWarehouse);
        }

        // POST: /Warehouse/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(Warehouse ObjWarehouse)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjWarehouse).State = EntityState.Modified;
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


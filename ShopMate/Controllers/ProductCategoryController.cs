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
    public class ProductCategoryController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        // GET: /ProductCategory/
        public ActionResult Index()
        {
            return View();
        }

        // GET ProductCategory/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.ProductCategorys.ToArray();
            var userwarehouse = db.Users.FirstOrDefault(i => i.UserName == userId).WarehouseId;
            if (userId =="Zimhope")
            {
                var result = from c in tak
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Name),
            Convert.ToString(c.ParentId),
             Convert.ToString(c.WarehouseId),
 };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = from c in tak.Where(m => m.WarehouseId == userwarehouse)
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Name),
            Convert.ToString(c.ParentId),
             Convert.ToString(c.WarehouseId),
 };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }
   
        }
        // GET: /ProductCategory/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: /ProductCategory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory ObjProductCategory = db.ProductCategorys.Find(id);
            if (ObjProductCategory == null)
            {
                return HttpNotFound();
            }
            return View(ObjProductCategory);
        }
        // GET: /ProductCategory/Create
        public ActionResult Create()
        {
            var userwarehouse = db.Users.FirstOrDefault(i => i.UserName == userId).WarehouseId;
            if(userId == "Zimhope")
            {
                ViewBag.ParentId = new SelectList(db.ProductCategorys, "Id", "Name");
                ViewBag.WareHouse = new SelectList(db.Warehouses, "Id", "Name");
            }
            else
            {
                ViewBag.ParentId = new SelectList(db.ProductCategorys.Where(i => i.WarehouseId == userwarehouse), "Id", "Name");
                ViewBag.WareHouse = new SelectList(db.Warehouses.Where(n => n.Id == userwarehouse), "Id", "Name");
            }
  

            return View();
        }

        // POST: /ProductCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(ProductCategory ObjProductCategory)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            var UserWarehouse = db.Users.FirstOrDefault(n => n.UserName == userId).WarehouseId;
            try
            {
                if (ModelState.IsValid)
                {

                    ObjProductCategory.WarehouseId = UserWarehouse;
                    db.ProductCategorys.Add(ObjProductCategory);
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
        // GET: /ProductCategory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory ObjProductCategory = db.ProductCategorys.Find(id);
            if (ObjProductCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.ProductCategorys, "Id", "Name", ObjProductCategory.ParentId);

            return View(ObjProductCategory);
        }

        // POST: /ProductCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(ProductCategory ObjProductCategory)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjProductCategory).State = EntityState.Modified;
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
        // GET: /ProductCategory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory ObjProductCategory = db.ProductCategorys.Find(id);
            if (ObjProductCategory == null)
            {
                return HttpNotFound();
            }
            return View(ObjProductCategory);
        }

        // POST: /ProductCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                ProductCategory ObjProductCategory = db.ProductCategorys.Find(id);
                db.ProductCategorys.Remove(ObjProductCategory);
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
        // GET: /ProductCategory/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            ProductCategory ObjProductCategory = db.ProductCategorys.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                ViewBag.ParentId = new SelectList(db.ProductCategorys, "Id", "Name", ObjProductCategory.ParentId);

            }

            return View(ObjProductCategory);
        }

        // POST: /ProductCategory/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(ProductCategory ObjProductCategory)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjProductCategory).State = EntityState.Modified;
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
        public ActionResult ProductGetGrid(int id = 0)
        {
            var tak = db.Products.Where(i => i.ProductCategoryId == id).ToArray();

            var result = from c in tak
                         select new string[] { Convert.ToString(c.Id),Convert.ToString(c.Id),
                Convert.ToString(c.Name),
                Convert.ToString(c. 	ProductCategoryId),
                Convert.ToString(c.BarCode),
                Convert.ToString(c.PurchasePrice),
                Convert.ToString(c.SalePrice),
                Convert.ToString(c.ProductImage),
                Convert.ToString(c.AddedBy),
                Convert.ToString(c.DateAdded),
                Convert.ToString(c.ModifiedBy),
                Convert.ToString(c.DateModied),
                Convert.ToString(c.IsActive),
                Convert.ToString(c.StockAlert),
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


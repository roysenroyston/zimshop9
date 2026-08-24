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
    public class InvoiceItemsController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        // GET: /InvoiceItems/
        public ActionResult Index()
        {
            return View();
        }
        
        // GET InvoiceItems/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.InvoiceItemss.ToArray();
             
            var result = from c in tak select new string[] { c.Id.ToString(), Convert.ToString(c.Id), 
            Convert.ToString(c.Product_ProductId.Name), 
            Convert.ToString(c.Quantity), 
            Convert.ToString(c.SalePrice), 
            Convert.ToString(c.TaxAmount), 
            Convert.ToString(c.TotalAmount), 
            Convert.ToString(c.TotalAmountWithTax), 
            Convert.ToString(c.AddedBy), 
            Convert.ToString(c.DateAdded), 
            Convert.ToString(c.TaxId), 
            Convert.ToString(c.PurchaseId), 
            Convert.ToString(c.SaleId), 
            Convert.ToString(c.ProductStockId), 
            Convert.ToString(c.TransactionId), 
            Convert.ToString(c.WarehouseId), 
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: /InvoiceItems/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: /InvoiceItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceItems ObjInvoiceItems = db.InvoiceItemss.Find(id);
            if (ObjInvoiceItems == null)
            {
                return HttpNotFound();
            }
            return View(ObjInvoiceItems);
        }
        // GET: /InvoiceItems/Create
        public ActionResult Create()
        {
             ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");

             return View();
        }

        // POST: /InvoiceItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(InvoiceItems ObjInvoiceItems )
        { 
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                if (ModelState.IsValid)
                { 
                    

                    db.InvoiceItemss.Add(ObjInvoiceItems);
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
        // GET: /InvoiceItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceItems ObjInvoiceItems = db.InvoiceItemss.Find(id);
            if (ObjInvoiceItems == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ObjInvoiceItems.ProductId);

            return View(ObjInvoiceItems);
        }

        // POST: /InvoiceItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(InvoiceItems ObjInvoiceItems )
        { 
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                if (ModelState.IsValid)
                { 
                    

                    db.Entry(ObjInvoiceItems).State = EntityState.Modified;
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
        // GET: /InvoiceItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceItems ObjInvoiceItems = db.InvoiceItemss.Find(id);
            if (ObjInvoiceItems == null)
            {
                return HttpNotFound();
            }
            return View(ObjInvoiceItems);
        }

        // POST: /InvoiceItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        { 
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                  
                    InvoiceItems ObjInvoiceItems = db.InvoiceItemss.Find(id);
                    db.InvoiceItemss.Remove(ObjInvoiceItems);
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
        // GET: /InvoiceItems/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        { 
            InvoiceItems ObjInvoiceItems = db.InvoiceItemss.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ObjInvoiceItems.ProductId);

            }
            
            return View(ObjInvoiceItems);
        }

        // POST: /InvoiceItems/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(InvoiceItems ObjInvoiceItems )
        {  
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                if (ModelState.IsValid)
                { 
                    

                    db.Entry(ObjInvoiceItems).State = EntityState.Modified;
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


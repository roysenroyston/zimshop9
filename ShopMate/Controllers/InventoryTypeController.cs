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
    public class InventoryTypeController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        // GET: /InventoryType/
        public ActionResult Index()
        {
            return View();
        }
        
        // GET InventoryType/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.InventoryTypes.ToArray();

            if (userId != "Zimhope")
            {
                var result = from c in tak.Where(n => n.Id == 10 ||n.Id == 11)
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Name),
             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            } else 
            {
                var result = from c in tak
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Name),
             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }
     
        }
        // GET: /InventoryType/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: /InventoryType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryType ObjInventoryType = db.InventoryTypes.Find(id);
            if (ObjInventoryType == null)
            {
                return HttpNotFound();
            }
            return View(ObjInventoryType);
        }
        // GET: /InventoryType/Create
        public ActionResult Create()
        {
             
             return View();
        }

        // POST: /InventoryType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(InventoryType ObjInventoryType )
        { 
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                if (ModelState.IsValid)
                { 
                    

                    db.InventoryTypes.Add(ObjInventoryType);
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
        // GET: /InventoryType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryType ObjInventoryType = db.InventoryTypes.Find(id);
            if (ObjInventoryType == null)
            {
                return HttpNotFound();
            }
            
            return View(ObjInventoryType);
        }

        // POST: /InventoryType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(InventoryType ObjInventoryType )
        { 
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                if (ModelState.IsValid)
                { 
                    

                    db.Entry(ObjInventoryType).State = EntityState.Modified;
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
        // GET: /InventoryType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryType ObjInventoryType = db.InventoryTypes.Find(id);
            if (ObjInventoryType == null)
            {
                return HttpNotFound();
            }
            return View(ObjInventoryType);
        }

        // POST: /InventoryType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        { 
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                  
                    InventoryType ObjInventoryType = db.InventoryTypes.Find(id);
                    db.InventoryTypes.Remove(ObjInventoryType);
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
        // GET: /InventoryType/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        { 
            InventoryType ObjInventoryType = db.InventoryTypes.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                
            }
            
            return View(ObjInventoryType);
        }

        // POST: /InventoryType/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(InventoryType ObjInventoryType )
        {  
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                if (ModelState.IsValid)
                { 
                    

                    db.Entry(ObjInventoryType).State = EntityState.Modified;
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


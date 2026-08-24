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
using TrackerEnabledDbContext.Common.Models;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class MyAudit : AuditLog
    {
        //  public string username  { get; set; }
    }

    public class SettingController : BaseController
    { 
        // GET: /Setting/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAuditGrid()
        {
            var tak = db.AuditLog.ToArray();

            var result = from c in tak
                         select new string[] {
                             c.AuditLogId.ToString(),
                             Convert.ToString(c.AuditLogId),
                             //Convert.ToString(db.Users.FirstOrDefault(i => i.Id == Int32.Parse(c.UserName)).UserName),
            Convert.ToString(c.UserName),
            Convert.ToString(c.EventDateUTC),
            Convert.ToString(c.EventType),
             Convert.ToString(c.TypeFullName),
             Convert.ToString(c.RecordId),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAuditlogDetailsGrid()
        {
            var tak = db.LogDetails.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.PropertyName),
            Convert.ToString(c.OriginalValue),
            Convert.ToString(c.NewValue),
             Convert.ToString(c.AuditLogId),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET Setting/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.Settings.ToArray();
             
            var result = from c in tak select new string[] { c.Id.ToString(), Convert.ToString(c.Id), 
            Convert.ToString(c.sKey), 
            Convert.ToString(c.sValue), 
            Convert.ToString(c.sGroup), 
             Convert.ToString(c.WarehouseId), 
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: /Setting/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: /Setting/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Setting ObjSetting = db.Settings.Find(id);
            if (ObjSetting == null)
            {
                return HttpNotFound();
            }
            return View(ObjSetting);
        }
        // GET: /Setting/Create
        public ActionResult Create()
        {
             
             return View();
        }

        // POST: /Setting/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Setting ObjSetting )
        { 
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                if (ModelState.IsValid)
                { 
                    

                    db.Settings.Add(ObjSetting);
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
        // GET: /Setting/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Setting ObjSetting = db.Settings.Find(id);
            if (ObjSetting == null)
            {
                return HttpNotFound();
            }
            
            return View(ObjSetting);
        }

        // POST: /Setting/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Setting ObjSetting )
        { 
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                if (ModelState.IsValid)
                { 
                    

                    db.Entry(ObjSetting).State = EntityState.Modified;
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
        // GET: /Setting/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Setting ObjSetting = db.Settings.Find(id);
            if (ObjSetting == null)
            {
                return HttpNotFound();
            }
            return View(ObjSetting);
        }

        // POST: /Setting/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        { 
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                  
                    Setting ObjSetting = db.Settings.Find(id);
                    db.Settings.Remove(ObjSetting);
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
        // GET: /Setting/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        { 
            Setting ObjSetting = db.Settings.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                
            }
            
            return View(ObjSetting);
        }

        // POST: /Setting/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(Setting ObjSetting )
        {  
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                if (ModelState.IsValid)
                { 
                    

                    db.Entry(ObjSetting).State = EntityState.Modified;
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


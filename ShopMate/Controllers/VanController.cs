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
    public class VanController : Controller
    {
        string userId = Env.GetUserInfo("name");
        private SIContext db = new SIContext();

        public ActionResult GetGrid()
        {
            var tak = db.Vans.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.RegNumber),
            Convert.ToString(c.IsActive),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: Van
        public ActionResult Index()
        {
            return View(db.Vans.ToList());
        }

        // GET: Van/Details/5
        public ActionResult GetDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Van ObjVan = db.Vans.Find(id);
            if (ObjVan == null)
            {
                return HttpNotFound();
            }
            return Json(new { RegNumber = ObjVan.RegNumber }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Van van = db.Vans.Find(id);
            if (van == null)
            {
                return HttpNotFound();
            }
            return View(van);
        }

        // GET: Van/Create
        public ActionResult Create()
        {
            ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            return View();
        }

        // POST: Van/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public ActionResult Create(Van ObjVan)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Van not added! The van already exists.";
            var tak = db.Vans.Where(i => i.RegNumber == ObjVan.RegNumber).FirstOrDefault();
            var tak2 = db.Users.Where(i => i.UserName == ObjVan.RegNumber).FirstOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (tak != null)
                    {
                        //if(tak2 != null) { }
                        //else
                        //{
                        //    User ObjUser = new User();
                        //    ObjUser.UserName = ObjVan.RegNumber;
                        //    ObjUser.FullName = ObjVan.RegNumber;
                        //    ObjUser.Password = "Password";
                        //    ObjUser.RoleId = db.Roles.FirstOrDefault(i => i.RoleName == "Customer").Id;
                        //    ObjUser.JoinDate = DateTime.Now;
                        //    ObjUser.IsActive = true;
                        //    ObjUser.CanOrder = false;
                        //    ObjUser.CanLogin = false;
                        //    ObjUser.WarehouseId = ObjVan.WarehouseId;
                        //    db.Users.Add(ObjUser);
                        //    db.SaveChanges();
                        //}
                        sb.Append("Van Already Exist");
                        result = "Van Already Exist";
                        //  return Content(sb.ToString());
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db.Vans.Add(ObjVan);
                        db.SaveChanges(userId);
                        //User ObjUser = new User();
                        //ObjUser.UserName = ObjVan.RegNumber;
                        //ObjUser.FullName = ObjVan.RegNumber;
                        //ObjUser.Password = "Password";
                        //ObjUser.RoleId = db.Roles.FirstOrDefault(i => i.RoleName == "Customer").Id;
                        //ObjUser.JoinDate = DateTime.Now;
                        //ObjUser.IsActive = true;
                        //ObjUser.CanOrder = false;
                        //ObjUser.CanLogin = false;
                        //ObjUser.WarehouseId = ObjVan.WarehouseId;
                        //db.Users.Add(ObjUser);
                        //db.SaveChanges();

                        sb.Append("Sumitted");
                        result = "Sumitted";

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

        // GET: Van/Edit/5
        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Van ObjVan = db.Vans.Find(id);
            if (ObjVan == null)
            {
                return HttpNotFound();
            }
            

            return View(ObjVan);
        }

        // POST: Van/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Van ObjVan)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            try
            {
                
                if (ModelState.IsValid)
                {


                    //ObjProduct.RemainingQuantity = ObjProduct.RemainingQuantity;
                    db.Entry(ObjVan).State = EntityState.Modified;
               

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

        // GET: Van/Delete/5
   

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Van ObjVan = db.Vans.Find(id);
            if (ObjVan == null)
            {
                return HttpNotFound();
            }
            return View(ObjVan);
        }

        // POST: Van/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                Van ObjVan = db.Vans.Find(id);
                db.Vans.Remove(ObjVan);
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
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}
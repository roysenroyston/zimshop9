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
    public class OrderController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        // GET Order/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.Orders.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),

            Convert.ToString(c.goods),
            Convert.ToString(c.supplier),
            Convert.ToString(c.purchasedate)

            };

            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }

        // GET: /Order/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }

        // GET: Order/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order ObjOrder = db.Orders.Find(id);
            if (ObjOrder == null)
            {
                return HttpNotFound();
            }
            return View(ObjOrder);
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.JobCards, "Id", "Name");
            ViewBag.userId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Supplier"), "Id", "UserName");

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Order ObjOrder, OrderMaterials[] OrderrMaterials)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Order Is Not Complete!";
            try
            {
                if (ModelState.IsValid)
                {
                    ObjOrder.purchasedate = DateTime.Now;
                    db.Orders.Add(ObjOrder);
                    db.SaveChanges();
                    foreach (var item in OrderrMaterials)
                    {
                        OrderMaterials materials = new OrderMaterials();
                        materials.Description = item.Description;
                        materials.Order = ObjOrder;
                        materials.OrderId = ObjOrder.Id;
                        materials.Quantity = item.Quantity;
                        db.OrderMaterial.Add(materials);
                        db.SaveChanges();
                    }


                    result = "Success! Order Is Completed!";
                    return Json(result, JsonRequestBehavior.AllowGet);
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

        // POST: /Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        //public ActionResult Create(Order ObjOrder)
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {


        //            db.Orders.Add(ObjOrder);
        //            db.SaveChanges(userId);

        //            sb.Append("Sumitted");
        //            return Content(sb.ToString());
        //        }
        //        else
        //        {
        //            foreach (var key in this.ViewData.ModelState.Keys)
        //            {
        //                foreach (var err in this.ViewData.ModelState[key].Errors)
        //                {
        //                    sb.Append(err.ErrorMessage + "<br/>");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        sb.Append("Error :" + ex.Message);
        //    }

        //    return Content(sb.ToString());

        //}

        // GET: Order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order ObjOrder = db.Orders.Find(id);
            if (ObjOrder == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.Orders, "Id", "Name", ObjOrder.Id);

            return View(ObjOrder);
        }

        // POST: Order/Edit/5        
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Order ObjOrder)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(ObjOrder).State = EntityState.Modified;
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

        // GET: Order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order ObjOrder = db.Orders.Find(id);
            if (ObjOrder == null)
            {
                return HttpNotFound();
            }
            return View(ObjOrder);
        }

        // POST: /Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                Order ObjOrder = db.Orders.Find(id);
                db.Orders.Remove(ObjOrder);
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

        // GET: /Order/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            Order ObjOrder = db.Orders.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                ViewBag.ParentId = new SelectList(db.Orders, "Id", "MenuText", ObjOrder.Id);

            }

            return View(ObjOrder);
        }

        // POST: /Order/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(Order ObjOrder)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjOrder).State = EntityState.Modified;
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

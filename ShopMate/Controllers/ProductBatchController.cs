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
    public class ProductBatchController : Controller
    {

            string userId = Env.GetUserInfo("name");
            private SIContext db = new SIContext();

            public ActionResult GetGrid()
            {
                var tak = db.ProductBatches.ToArray();

                var result = from c in tak
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(db.Products.FirstOrDefault(i => i.Id == c.ProductId).Name),
            //Convert.ToString(c.ProductId),
            Convert.ToString(c.BatchNumber),
            Convert.ToString(c.ExpiryDate),
             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }
            // GET: Banks
            public ActionResult Index()
            {
                return View(db.ProductBatches.ToList());
            }

            // GET: Banks/Details/5
            public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            ProductBatch productBatch = db.ProductBatches.Find(id);
                if (productBatch == null)
                {
                    return HttpNotFound();
                }
                return View(productBatch);
            }

        // GET: Banks/Create
        public ActionResult Create()
        {
            var userwarehouse = db.Users.FirstOrDefault(n => n.UserName == userId).WarehouseId;
            if(userId == "Zimhope")
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == userwarehouse), "Id", "Name");
            }

            //ViewBag.InventoryTypeId = new SelectList(db.InventoryTypes, "Id", "Name");
            //ViewBag.taxId = new SelectList(db.Taxs, "Id", "Name");
            return View();
        }


        [HttpPost]
            [ValidateAntiForgeryToken]

        public ActionResult Create(ProductBatch ObjProductBatch)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Bank not added! The bank already exists.";
            var tak = db.ProductBatches.Where(i => i.BatchNumber == ObjProductBatch.BatchNumber && i.ProductId == ObjProductBatch.ProductId).FirstOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (tak != null)
                    {
                        sb.Append("Batch Already Exist");
                        result = "Batch Already Exist";

                    }
                    else
                    {
                        db.ProductBatches.Add(ObjProductBatch);
                        db.SaveChanges(userId);


                        sb.Append("Sumitted");
                       

                    }

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

            //sb.Append("Submitted");
            return Content(sb.ToString());

        }



        //public ActionResult Create(ProductBatch ObjProductBatch, HttpPostedFileBase ProductImage, string HideImage1)
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    string result = "Error! Product not added! The Product already exist.";
        //    //var tak = db.Products.Where(i => i.Name == ObjProduct.Name && i.ProductCategoryId == ObjProduct.ProductCategoryId).FirstOrDefault();
        //    var tak = db.ProductBatches.Where(i => i.BatchNumber == ObjProductBatch.BatchNumber && i.ProductId == ObjProductBatch.ProductId).FirstOrDefault();

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            if (ProductImage != null)
        //            {
        //                var fileName = MicrosoftHelper.MSHelper.StarkFileUploaderCSharp(ProductImage, Server.MapPath("~/Uploads"));
        //                ModelState.Clear();
                        
        //            }
        //            else
        //            {
        //                ModelState.Clear();
        //            }

        //            if (tak != null)
        //            {
        //                sb.Append("Batch Already Exist");
        //                //result = "Batch Already Exist";

        //                //sb.Append("Product Already Exist ");
        //                return Content(sb.ToString());
        //                // return Json(result, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
                      
        //                db.ProductBatches.Add(ObjProductBatch);
        //                db.SaveChanges(userId);
        //                sb.Append("Submitted");

        //            }


                    
        //            //return Content(sb.ToString());
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
        //        Helper.WriteError(ex, ex.Message);
        //        sb.Append("Error :" + ex.Message);
        //    }

        //    return Content(sb.ToString());

        //}













        // GET: Banks/Edit/5
        public ActionResult Edit(int? id)
            {
               
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ProductBatch ObjProductBatch = db.ProductBatches.Find(id);
                if (ObjProductBatch == null)
                {
                    return HttpNotFound();
                }
               
                return View(ObjProductBatch);
            }

            
            [HttpPost]
            [ValidateAntiForgeryToken]
            [ValidateInput(false)]
            public ActionResult Edit(ProductBatch ObjProductBatch)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                try
                {
                    
                    if (ModelState.IsValid)
                    {


                        db.Entry(ObjProductBatch).State = EntityState.Modified;
                      

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

            
            public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ProductBatch ObjProductBatch = db.ProductBatches.Find(id);
                if (ObjProductBatch == null)
                {
                    return HttpNotFound();
                }
                return View(ObjProductBatch);
            }

            // POST: Banks/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                try
                {

                ProductBatch ObjProductBatch = db.ProductBatches.Find(id);
                    db.ProductBatches.Remove(ObjProductBatch);
                    db.SaveChanges(userId);

                sb.Append("Submitted");
                //return View();
                //return Content(sb.ToString());
                //return View(result);

                return View("Index");

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


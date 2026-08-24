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
    public class RawMaterialsController : Controller
    {
        string userId = Env.GetUserInfo("name");
        private SIContext db = new SIContext();
        // GET: /Product/
        public ActionResult Index() 
        {
            
            return View();
        }
        public ActionResult GetGrid()
        {
            try
            {
                var tak = db.RawMaterial.ToArray();
                // var tax =  db.Taxs;
                var tax = db.Taxs.ToArray();

                var result = from c in tak
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Name),
            Convert.ToString(c.AddedBy),
            Convert.ToString(c.StockAlert),
            Convert.ToString(c.WarehouseId),
            Convert.ToString(c.DateAdded),
            Convert.ToString(tax.FirstOrDefault(i=>i.Id==c.TaxId).TaxRate+" %")
             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }
            catch (NullReferenceException ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View(ex.Message);
            }
        }
        // GET: RawMaterials/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RawMaterials ObjRawMaterials = db.RawMaterial.Find(id);
            if (ObjRawMaterials == null)
            {
                return HttpNotFound();
            }
            ViewBag.taxId = Convert.ToString(db.Taxs.FirstOrDefault(i => i.Id == (ObjRawMaterials.TaxId)).Name);
            
            //return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            return View(ObjRawMaterials);
        }


        // GET: RawMaterials/Create
        public ActionResult Create()
        {
            ViewBag.TaxId = new SelectList(db.Taxs, "Id", "Name");
            return View();
        }

        // POST: /Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(RawMaterials ObjProduct)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Raw Material not added! The Material already exist.";
            var tak = db.RawMaterial.Where(i => i.Name == ObjProduct.Name).FirstOrDefault();

            try
            {
                if (ModelState.IsValid)
                {
                    if (tak != null)
                    {
                        sb.Append("Material Already Exist ");
                        result = "Material Already Exist";
                      //  return Content(sb.ToString());
                         return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db.RawMaterial.Add(ObjProduct);
                        db.SaveChanges(userId);

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
        // GET: RawMaterials/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RawMaterials ObjProduct = db.RawMaterial.Find(id);
            if (ObjProduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.TaxId = new SelectList(db.Taxs, "Id", "Name", ObjProduct.TaxId);

            return View(ObjProduct);
        }

        // POST: /Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(RawMaterials ObjProduct)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            try
            {
                //  List<ProductStock> stock = db.ProductStocks.Where(i=>i.Id==ObjProduct.ProductStock_ProductIds);
                if (ModelState.IsValid)
                {
                    

                    //ObjProduct.RemainingQuantity = ObjProduct.RemainingQuantity;
                    db.Entry(ObjProduct).State = EntityState.Modified;
                    // db.SaveChanges();
                    int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
                    // var selectedProduct = db.Products.FirstOrDefault(i => i.Id == ObjPurchase.ProductId);
                    var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == ObjProduct.TaxId);

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

        // GET: RawMaterials/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RawMaterials ObjProduct = db.RawMaterial.Find(id);
            if (ObjProduct == null)
            {
                return HttpNotFound();
            }
            return View(ObjProduct);
        }

        // POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                RawMaterials ObjProduct = db.RawMaterial.Find(id);
                db.RawMaterial.Remove(ObjProduct);
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
    }
  
}

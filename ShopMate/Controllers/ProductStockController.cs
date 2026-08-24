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
    public class ProductStockController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
        int AddedBy = int.Parse(Env.GetUserInfo("userid"));
        // GET: /ProductStock/
        public ActionResult Index()
        {
            //ViewBag.ProductId = new SelectList(db.ProductStocks, "Id", "Name");
            return View();
        }

        // GET ProductStock/GetGrid
        public ActionResult GetGrid()
        {
            try
            {
                int wareid = Convert.ToInt16(warehouse);
                var tak = db.ProductStock.Where(n => n.InventoryTypeId == 10 || n.InventoryTypeId == 11).ToArray();
                var user = db.Users.ToArray();
                var tax = db.Taxs.ToArray();


                // var username = db.Users.FirstOrDefault(n => n.FullName == userId).WarehouseId;

                var result = from c in tak.Where(n => n.WarehouseId == wareid)
                             select new string[] {
                            c.Id.ToString(),
                            Convert.ToString(c.Id),
            Convert.ToString(c.Product_ProductId.Name),
            Convert.ToString(c.Quantity),
            //Convert.ToString(c.PurchasePrice),
            //Convert.ToString(c.TotalPurchaseAmount),
            //Convert.ToString(c.TotalSaleAmount),
            //Convert.ToString(c.TotalSaleAmountWithTax),
            //Convert.ToString(c.Discount),
            Convert.ToString(c.TaxId),
            Convert.ToString(c.Description),
            Convert.ToString(c.AddedBy),
            Convert.ToString(c.DateAdded),
            Convert.ToString(c.ModifiedBy),
            Convert.ToString(c.DateModied),
            Convert.ToString(c.InventoryType_InventoryTypeId.Name),
            Convert.ToString(db.Warehouses.FirstOrDefault( w => w.Id == c.WarehouseId).Name)
            };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View(ex.Message);
            }

            //bool isAdmin = false;
            ////TODO: Check the user if it is admin or normal user, (true-Admin, false- Normal user)
            //string output = isAdmin ? "Welcome to the Admin User" : "Welcome to the User";

            //return Json(output, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: /ProductStock/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductStock ObjProductStock = db.ProductStock.Find(id);
            if (ObjProductStock == null)
            {
                return HttpNotFound();
            }
            return View(ObjProductStock);
        }

        // GET: /ProductStock/Create
        public ActionResult Create()
        {
            int userWarehouseId = warehouse;

            {
                ViewBag.WareHouse = new SelectList(db.Warehouses.Where(n => n.Id == userWarehouseId), "Id", "Name");
                ViewBag.ProductId = new SelectList(db.Products.Where(b => b.WarehouseId == userWarehouseId && b.IsActive == true), "Id", "Name");
                ViewBag.InventoryTypeId = new SelectList(db.InventoryTypes.Where(n => n.Id == 10 || n.Id == 11), "Id", "Name");
                ViewBag.taxId = new SelectList(db.Taxs, "Id", "Name");
                ViewBag.ProductBatchId = new SelectList(db.ProductBatches, "Id", "BatchNumber");
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(m => m.Id == userWarehouseId), "Id", "Name");
            }

            return View();
        }

        // POST: /ProductStock/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        //    public ActionResult Create(ProductStock ObjProductStock)
        public ActionResult Create(/*ProductStock ObjProductStock,*/ int? InvenoryId, ProductStock[] productss, int? WarehouseId, string Description = "")
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Adjustment  Is Not Complete!";
            //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));

            //var selectedProduct = db.Products.FirstOrDefault(i => i.Id == ObjProductStock.ProductId);
            //var selectedWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == ObjProductStock.ProductId && i.WarehouseId == warehouse);
            //var ObjWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == ObjProductStock.ProductId && i.WarehouseId == warehouse);//ngoni

            try
            {
                if (ModelState.IsValid)
                {
                    foreach (var item in productss)
                    {
                        ProductStock ObjProductStock = new ProductStock();
                        ObjProductStock.ProductId = item.ProductId;
                        ObjProductStock.InventoryTypeId = InvenoryId;
                        ObjProductStock.WarehouseId = warehouse;
                        ObjProductStock.Description = Description;
                        ObjProductStock.Quantity = item.Quantity;
                        ObjProductStock.AddedBy = AddedBy;
                        ObjProductStock.DateAdded = DateTime.Now;
                        ObjProductStock.DateModied = DateTime.Now;
                        ObjProductStock.ModifiedBy = AddedBy;

                        //        ObjProductStock.ProductBatchId = db.ProductBatches.FirstOrDefault(m => m.BatchNumber == "Sale").Id;
                        //ObjProductStock.WarehouseId = warehouse;
                        //db.ProductStock.Add(ObjProductStock);
                        //db.SaveChanges(userId);

                        var selectedProduct = db.Products.FirstOrDefault(i => i.Id == ObjProductStock.ProductId);
                        var selectedWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == ObjProductStock.ProductId && i.WarehouseId == ObjProductStock.WarehouseId);//ngoni

                        if (selectedProduct == null)
                        {
                            result = " Error! Adjustment  Is Not Complete! " + selectedProduct.Name + " Error! selected Product is Not Available ";
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        else if (selectedWarehouseStock == null)
                        {
                            result = " Error! Adjustment  Is Not Complete! " + selectedProduct.Name + " Product is not in the selected WarehouseStock ";
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }



                        if (ObjProductStock.InventoryTypeId == 1 || ObjProductStock.InventoryTypeId == 2)
                        {
                            sb.Append("Cannot Make sale or Purchase From here!!");
                        }
                        else if (ObjProductStock.InventoryTypeId == 5)
                        {
                            if (selectedProduct.RemainingQuantity < ObjProductStock.Quantity)
                            {
                                sb.Append("Remaining Quantity not enough to perform action");
                                return Content(sb.ToString());
                            }
                            else
                            {
                                selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ObjProductStock.Quantity;
                                selectedProduct.RemainingAmount = selectedProduct.RemainingAmount - ObjProductStock.TotalSaleAmountWithTax;
                                db.Entry(selectedProduct).State = EntityState.Modified;
                                db.SaveChanges(userId);
                            }
                        }
                        else if (ObjProductStock.InventoryTypeId == db.InventoryTypes.FirstOrDefault(i => i.Name == "Expired").Id)
                        {

                            selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ObjProductStock.Quantity;
                            selectedProduct.RemainingAmount = selectedProduct.RemainingAmount - ObjProductStock.TotalSaleAmountWithTax;
                            //var selectedBatch = db.ProductBatches.FirstOrDefault(i => i.Id == ObjProductStock.ProductBatchId);
                            //if (selectedBatch != null)
                            //{
                            //    selectedBatch.IsCleared = true;
                            //    db.Entry(selectedBatch).State = EntityState.Modified;
                            //}
                            db.Entry(selectedProduct).State = EntityState.Modified;
                            db.SaveChanges(userId);

                        }
                        else if (ObjProductStock.InventoryTypeId == 6)
                        {
                            //selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity + ObjProductStock.Quantity;
                            //selectedProduct.RemainingAmount = selectedProduct.RemainingAmount + ObjProductStock.TotalSaleAmountWithTax;
                            //db.Entry(selectedProduct).State = EntityState.Modified;
                            //db.SaveChanges(userId);
                        }
                        else if (ObjProductStock.InventoryTypeId == 7)
                        {
                            if (selectedWarehouseStock == null)
                            {
                                WarehouseStock wstock = new WarehouseStock();
                                wstock.ProductId = ObjProductStock.ProductId;
                                wstock.RemainingQuantity = 0;
                                wstock.WarehouseId = ObjProductStock.WarehouseId;
                                db.WarehouseStocks.Add(wstock);
                                db.SaveChanges(userId);
                                selectedWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.Id == wstock.Id);
                            }
                            if (selectedProduct.RemainingQuantity < ObjProductStock.Quantity)
                            {
                                sb.Append("Remaining Quantity not enough to perform action");
                                return Content(sb.ToString());
                            }
                            else
                            {
                                selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ObjProductStock.Quantity;
                                selectedProduct.RemainingAmount = selectedProduct.RemainingAmount - ObjProductStock.TotalSaleAmountWithTax;
                                db.Entry(selectedProduct).State = EntityState.Modified;

                                selectedWarehouseStock.RemainingQuantity = selectedWarehouseStock.RemainingQuantity + ObjProductStock.Quantity;
                                db.Entry(selectedWarehouseStock).State = EntityState.Modified;

                                db.SaveChanges(userId);
                            }
                        }
                        else if (ObjProductStock.InventoryTypeId == 3)
                        {
                            selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ObjProductStock.Quantity;
                            selectedProduct.RemainingAmount = selectedProduct.RemainingAmount - ObjProductStock.TotalSaleAmountWithTax;
                            db.Entry(selectedProduct).State = EntityState.Modified;
                            db.SaveChanges(userId);

                        }
                        else if (ObjProductStock.InventoryTypeId == 4)
                        {
                            //selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity + ObjProductStock.Quantity;
                            //selectedProduct.RemainingAmount = selectedProduct.RemainingAmount + ObjProductStock.TotalSaleAmountWithTax;
                            selectedWarehouseStock.RemainingQuantity = selectedWarehouseStock.RemainingQuantity + ObjProductStock.Quantity;
                            //db.Entry(selectedProduct).State = EntityState.Modified;
                            db.Entry(selectedWarehouseStock).State = EntityState.Modified;//ngonie
                            db.SaveChanges(userId);
                        }
                        else if (ObjProductStock.InventoryTypeId == 10)
                        {

                            selectedWarehouseStock.RemainingQuantity = selectedWarehouseStock.RemainingQuantity + ObjProductStock.Quantity;
                            db.Entry(selectedWarehouseStock).State = EntityState.Modified;//ngonie
                            db.SaveChanges(userId);
                        }
                        else if (ObjProductStock.InventoryTypeId == 11)
                        {

                            if (selectedWarehouseStock.RemainingQuantity > 0)
                            {
                                if (selectedWarehouseStock.RemainingQuantity >= item.Quantity)
                                {
                                    selectedWarehouseStock.RemainingQuantity = selectedWarehouseStock.RemainingQuantity - ObjProductStock.Quantity;
                                    db.Entry(selectedWarehouseStock).State = EntityState.Modified;//ngonie
                                    db.SaveChanges(userId);
                                }
                                else
                                {
                                    result = " Error! Adjustment Is Not Complete! Adjusted Qty " + item.Quantity + " is greater than Remaning Qty, " + selectedWarehouseStock.RemainingQuantity;
                                    return Json(result, JsonRequestBehavior.AllowGet);
                                }

                            }
                            else
                            {

                                result = " Error! Adjustment Is Not Complete Remaining Qty  " + selectedWarehouseStock.RemainingQuantity + " is less than Adjusted Qty, " + item.Quantity;
                                //result = " Error! Adjustment  Is Not Complete!" + selectedProduct.Name + "Product is not in the selected WarehouseStock ";
                                return Json(result, JsonRequestBehavior.AllowGet);
                            }


                            ////selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity + ObjProductStock.Quantity;
                            ////selectedProduct.RemainingAmount = selectedProduct.RemainingAmount + ObjProductStock.TotalSaleAmountWithTax;
                            //selectedWarehouseStock.RemainingQuantity = selectedWarehouseStock.RemainingQuantity - ObjProductStock.Quantity;
                            ////db.Entry(selectedProduct).State = EntityState.Modified;
                            //db.Entry(selectedWarehouseStock).State = EntityState.Modified;//ngonie
                            //db.SaveChanges(userId);
                        }

                        ObjProductStock.PurchasePrice = selectedProduct.PurchasePrice;
                        ObjProductStock.SalePrice = selectedProduct.SalePrice;
                        ObjProductStock.RemainingQuantity = selectedWarehouseStock.RemainingQuantity;
                        db.ProductStock.Add(ObjProductStock);
                        db.SaveChanges(userId);
                    }

                    result = "Success! Adjustment Completed";
                    return Json(result, JsonRequestBehavior.AllowGet);
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
            //result = "Success! Adjustment Completed";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // GET: /ProductStock/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductStock ObjProductStock = db.ProductStock.Find(id);
            if (ObjProductStock == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ObjProductStock.ProductId);
            ViewBag.InventoryTypeId = new SelectList(db.InventoryTypes, "Id", "Name", ObjProductStock.InventoryTypeId);

            return View(ObjProductStock);
        }

        // POST: /ProductStock/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(ProductStock ObjProductStock)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjProductStock).State = EntityState.Modified;
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
        // GET: /ProductStock/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductStock ObjProductStock = db.ProductStock.Find(id);
            if (ObjProductStock == null)
            {
                return HttpNotFound();
            }
            return View(ObjProductStock);
        }

        // POST: /ProductStock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                ProductStock ObjProductStock = db.ProductStock.Find(id);
                db.ProductStock.Remove(ObjProductStock);
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
        // GET: /ProductStock/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            ProductStock ObjProductStock = db.ProductStock.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ObjProductStock.ProductId);
                ViewBag.InventoryTypeId = new SelectList(db.InventoryTypes, "Id", "Name", ObjProductStock.InventoryTypeId);

            }

            return View(ObjProductStock);
        }

        // POST: /ProductStock/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(ProductStock ObjProductStock)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjProductStock).State = EntityState.Modified;
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


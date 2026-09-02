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
    public class SaleController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        int warehouses = int.Parse(Env.GetUserInfo("WarehouseId"));
        // GET: /Sale/
        public ActionResult Index()
        {
            return View();
        }

        // GET Sale/GetGrid?days=30  (defaults to last 30 days — pass days=0 for all records)
        public ActionResult GetGrid(int days = 30)
        {
            var userwarehouse = db.Users.FirstOrDefault(n => n.UserName == userId).WarehouseId;
            var query = db.Sales.Where(n => n.WarehouseId == userwarehouse);

            if (days > 0)
            {
                var cutoff = DateTime.Now.Date.AddDays(-days);
                query = query.Where(n => n.DateAdded >= cutoff);
            }

            var tak = query.OrderByDescending(n => n.DateAdded).ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
                         Convert.ToString(c.recieptNumber),
                      Convert.ToString(c.Product_ProductId.Name),
                            Convert.ToString(c.Quantity),
                             Convert.ToString(c.ReturnedQuantity),
                            Convert.ToString(c.CustomerUserId),
                            Convert.ToString(c.UnitPrice),
                            Convert.ToString(c.SalePrice),
                            Convert.ToString(c.RtgsPrice),
                            Convert.ToString(c.PaymentMode_PaymentModeId.Name),
                            Convert.ToString(c.TotalAmount),
                            Convert.ToString(c.TotalRtgsAmount),
                            Convert.ToString(c.DateAdded),
                            Convert.ToString(c.AddedBy) ,
                            Convert.ToString(c.InventoryTypeId)
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);


        }
        // GET: /Sale/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: /Sale/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale ObjSale = db.Sales.Find(id);
            if (ObjSale == null)
            {
                return HttpNotFound();
            }
            return View(ObjSale);
        }
        // GET: /Sale/Create
        public ActionResult Create()
        {
            ViewBag.CustomerUserId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Customer"), "Id", "UserName");
            ViewBag.PaymentModeId = new SelectList(db.PaymentModes, "Id", "Name");
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");

            //var tax = db.Taxs.ToArray();
            //ViewBag.TaxId = new SelectList(tax, "Id", "Name"); 
            return View();
        }

        // POST: /Sale/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Sale ObjSale, string Description = "")
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {

                    string CustomerName = db.Users.FirstOrDefault(i => i.Id == ObjSale.CustomerUserId).UserName;

                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
                            var selectedProduct = db.Products.FirstOrDefault(i => i.Id == ObjSale.ProductId);
                            var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == selectedProduct.TaxId);

                            if (selectedProduct.RemainingQuantity >= ObjSale.Quantity)
                            {

                                ObjSale.SalePrice = selectedProduct.SalePrice;
                                ObjSale.TotalAmount = (selectedProduct.SalePrice * ObjSale.Quantity);
                                ObjSale.WarehouseId = warehouse;

                                db.Sales.Add(ObjSale);
                                db.SaveChanges(userId);

                                //ProductStock begin
                                ProductStock ps = new ProductStock();
                                ps.ProductId = ObjSale.ProductId;
                                ps.Quantity = ObjSale.Quantity;

                                ps.PurchasePrice = selectedProduct.PurchasePrice;
                                ps.TotalPurchaseAmount = (selectedProduct.PurchasePrice * ObjSale.Quantity);
                                ps.SalePrice = selectedProduct.SalePrice;
                                ps.Discount = selectedProduct.Discount;
                                ps.TotalSaleAmount = (selectedProduct.SalePrice * ObjSale.Quantity);

                                decimal TaxAmount = 0;
                                //if (selectedTax.Other == "GST")
                                //{
                                //    decimal taxSplit = selectedTax.TaxRate / 2;
                                //    ps.CGST = selectedProduct.TaxId;
                                //    ps.CGST_Rate = taxSplit;
                                //    ps.SGST = selectedProduct.TaxId;
                                //    ps.SGST_Rate = taxSplit;

                                //    TaxAmount = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                                //}
                                //else if (selectedTax.Other == "IGST")
                                //{
                                //    ps.IGST = selectedProduct.TaxId;
                                //    ps.IGST_Rate = selectedTax.TaxRate;

                                //    TaxAmount = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                                //}
                                //else if (selectedTax.Other == "Other")
                                //{
                                ps.TaxId = selectedProduct.TaxId;
                                //    ps.OtherTaxValue = selectedTax.TaxRate;
                                //    TaxAmount = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                                //}



                                ps.TotalSaleAmountWithTax = (selectedProduct.SalePrice * ObjSale.Quantity) + TaxAmount;
                                ps.TaxAmount = TaxAmount;
                                //ps.Profit = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);
                                ps.Profit = (ps.TotalSaleAmount * ps.TotalPurchaseAmount);
                                ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount) + TaxAmount;

                                ps.Description = Description;
                                ps.AddedBy = ObjSale.AddedBy;
                                ps.DateAdded = DateTime.Now;
                                ps.ModifiedBy = ObjSale.AddedBy;
                                ps.DateModied = DateTime.Now;
                                ps.InventoryTypeId = 2;
                                ps.WarehouseId = warehouse;
                                db.ProductStock.Add(ps);
                                //end
                                db.SaveChanges(userId);
                                //Get Ledger Account
                                int vendorLedger = 0;

                                var LedgerA = db.LedgerAccounts.FirstOrDefault(i => i.Name.Trim() == CustomerName.Trim());
                                if (LedgerA != null)
                                {
                                    vendorLedger = LedgerA.Id;
                                }
                                else
                                {
                                    LedgerAccount la = new LedgerAccount();
                                    la.Name = CustomerName.Trim();
                                    la.ParentId = 12;
                                    la.AddedBy = ObjSale.AddedBy;
                                    la.DateAdded = DateTime.Now;
                                    db.LedgerAccounts.Add(la);

                                    vendorLedger = la.Id;
                                }
                                //end 

                                //transaction
                                Transaction tr = new Transaction();
                                tr.AddedBy = ObjSale.AddedBy;
                                tr.DebitLedgerAccountId = vendorLedger;
                                tr.DebitAmount = (ps.TotalPurchaseAmount + TaxAmount);
                                tr.CreditLedgerAccountId = 11;
                                tr.CreditAmount = (ps.TotalPurchaseAmount + TaxAmount);
                                tr.DateAdded = DateTime.Now;
                                tr.Remarks = "Sale, Sale Account credit and " + CustomerName + " account debit";
                                tr.Other = null;
                                tr.PurchaseOrSale = "Sale";
                                tr.PurchaseIdOrSaleId = ObjSale.Id;
                                tr.WarehouseId = warehouse;
                                db.Transactions.Add(tr);
                                //end

                                db.SaveChanges(userId);

                                Invoice inv = new Invoice();
                                inv.AddedBy = ObjSale.AddedBy;
                                inv.DateAdded = DateTime.Now;
                                inv.DateModied = DateTime.Now;
                                inv.IsBilled = false;
                                inv.IsPurchaseOrSale = "Sale";
                                inv.ModifiedBy = ObjSale.AddedBy;
                                inv.UserId = ObjSale.CustomerUserId.Value;
                                inv.WarehouseId = warehouse;
                                db.Invoices.Add(inv);

                                db.SaveChanges(userId);

                                InvoiceItems Iitem = new InvoiceItems(); // you should play around with this, this is where a new invoice item is instantiated

                                Iitem.ProductId = ObjSale.ProductId;
                                Iitem.Quantity = ObjSale.Quantity;
                                Iitem.TaxAmount = TaxAmount;
                                Iitem.AddedBy = ObjSale.AddedBy;
                                Iitem.DateAdded = DateTime.Now;
                                Iitem.SalePrice = selectedProduct.SalePrice;
                                Iitem.TotalAmount = ps.TotalSaleAmount;
                                Iitem.TotalAmountWithTax = ps.TotalSaleAmountWithTax;
                                Iitem.TaxId = selectedProduct.TaxId;
                                Iitem.PurchaseId = null;
                                Iitem.SaleId = ObjSale.Id; ;
                                Iitem.ProductStockId = ps.Id;
                                Iitem.TransactionId = tr.Id;
                                Iitem.WarehouseId = warehouse;
                                Iitem.InvoiceId = inv.Id;
                                db.InvoiceItemss.Add(Iitem);

                                db.SaveChanges(userId);
                                //okay boy let me look at it, but its not what i asked, i need a way to test it in clientside before it is even created, 
                                // there is no where way you can test your client without returning the correct data.. put debuggers the same way i showed and understand how the code is functioning even from developer tools
                                selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ObjSale.Quantity; // here as well
                                selectedProduct.RemainingAmount = selectedProduct.RemainingAmount - ps.TotalSaleAmountWithTax;
                                db.Entry(selectedProduct).State = EntityState.Modified;
                                db.SaveChanges(userId);

                                dbContextTransaction.Commit();
                            }
                            else
                            {
                                if (selectedProduct.RemainingQuantity > 0)
                                {
                                    sb.Append("Error : Product Stock Quantity Is " + selectedProduct.RemainingQuantity + " Low So Reduce your Quantity");
                                }
                                else
                                {
                                    sb.Append("Error : Product Stock Quantity Is " + selectedProduct.RemainingQuantity + " Low So Purchase First");
                                }
                                return Content(sb.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Helper.WriteError(ex, ex.Message);
                            dbContextTransaction.Rollback();
                        }
                    }


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
        // GET: /Sale/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale ObjSale = db.Sales.Find(id);
            if (ObjSale == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerUserId = new SelectList(db.Users, "Id", "UserName", ObjSale.CustomerUserId);
            ViewBag.PaymentModeId = new SelectList(db.PaymentModes, "Id", "Name", ObjSale.PaymentModeId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ObjSale.ProductId);

            return View(ObjSale);
        }

        // POST: /Sale/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Sale ObjSale)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjSale).State = EntityState.Modified;
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
        // GET: /Sale/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sale ObjSale = db.Sales.Find(id);
            if (ObjSale == null)
            {
                return HttpNotFound();
            }
            return View(ObjSale);
        }

        // POST: /Sale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                Sale ObjSale = db.Sales.Find(id);

                InvoiceItems ObjInvoiceItems = db.InvoiceItemss.FirstOrDefault(i => i.SaleId == ObjSale.Id);

                Invoice ObjInvoice = db.Invoices.FirstOrDefault(i => i.Id == ObjInvoiceItems.InvoiceId);


                ProductStock ObjProductStock = db.ProductStock.FirstOrDefault(i => i.Id == ObjInvoiceItems.ProductStockId);

                Transaction ObjTransaction = db.Transactions.FirstOrDefault(i => i.Id == ObjInvoiceItems.TransactionId);

                var selectedProduct = db.Products.FirstOrDefault(i => i.Id == ObjSale.ProductId);

                selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity + ObjSale.Quantity;
                selectedProduct.RemainingAmount = selectedProduct.RemainingAmount + (ObjProductStock.TotalSaleAmountWithTax);

                db.Entry(selectedProduct).State = EntityState.Modified;
                db.SaveChanges(userId);


                db.Sales.Remove(ObjSale);

                db.ProductStock.Remove(ObjProductStock);

                db.Transactions.Remove(ObjTransaction);

                db.InvoiceItemss.Remove(ObjInvoiceItems);

                db.Invoices.Remove(ObjInvoice);

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




        [HttpGet]
        public ActionResult SaleReturn(int id)
        {
            try
            {
                ViewBag.qty = Request.QueryString["qty"].ToString();
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
            }

            ViewBag.purchaseid = id;
            return View();
        }

        [HttpPost]
        public ActionResult SaleReturn(decimal Quantity, int? id)
        {
            //algorithem
            ///if full quantity return than
            ///sale (Just change inventorytypeid)
            ///ProductStock (Just change inventorytypeid)
            ///[Transaction] (make reverse new entry) :: 
            ///
            ///
            var AddedBy = db.Sales.FirstOrDefault(i => i.Id == id).AddedBy;
            //var ModifiedBy = db.Sales.FirstOrDefault(i => i. == id).AddedBy;
            int ModifiedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            // int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var warehouse = db.Users.FirstOrDefault(i => i.Id == AddedBy).WarehouseId;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                Sale ObjSale = db.Sales.Find(id);

                if (ObjSale.Quantity >= ObjSale.ReturnedQuantity)
                {





                    var selectedProduct = db.Products.FirstOrDefault(i => i.Id == ObjSale.ProductId);

                    var ObjWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == ObjSale.ProductId && i.WarehouseId == ObjSale.WarehouseId);

                    selectedProduct.RemainingAmount = selectedProduct.RemainingAmount + (ObjSale.SalePrice * Quantity);



                    ObjSale.ModifiedBy = ModifiedBy;
                    ObjSale.DateModied = DateTime.Now;
                    ObjSale.ReturnedQuantity = ObjSale.ReturnedQuantity + Quantity;
                    ObjSale.TotalAmount = ObjSale.TotalAmount - (ObjSale.SalePrice * Quantity);
                    ObjSale.PaidAmount = ObjSale.PaidAmount - (ObjSale.SalePrice * Quantity);
                    ObjSale.Quantity = ObjSale.Quantity - Quantity;
                    ObjSale.TotalAmountWithTax = ObjSale.TotalAmountWithTax - (ObjSale.SalePrice * Quantity);
                    db.Entry(ObjSale).State = EntityState.Modified;




                    var profreturn = db.ProductStock.FirstOrDefault(n => n.SaleId == id);
                    if (profreturn != null)
                    {
                        ProductStock prof = db.ProductStock.FirstOrDefault(m => m.SaleId == id);
                        prof.TotalPurchaseAmount = prof.TotalPurchaseAmount - (selectedProduct.PurchasePrice * Quantity);
                        prof.TotalSaleAmount = prof.TotalSaleAmount - (ObjSale.SalePrice * Quantity);

                        decimal myprofit = (selectedProduct.SalePrice * Quantity) - (selectedProduct.PurchasePrice * Quantity);
                        prof.Profit = prof.Profit - myprofit;
                        db.Entry(prof).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    WarehouseStock warehse = new WarehouseStock();
                    warehse = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == ObjSale.ProductId && i.WarehouseId == ObjSale.WarehouseId);
                    warehse.RemainingQuantity = ObjWarehouseStock.RemainingQuantity + Quantity;
                    db.Entry(warehse).State = EntityState.Modified;
                    db.SaveChanges();

                    ProductStock ps = new ProductStock();
                    ps.ProductId = ObjSale.ProductId;
                    ps.Quantity = Quantity;

                    ps.PurchasePrice = selectedProduct.PurchasePrice;

                    ps.TotalPurchaseAmount = (selectedProduct.PurchasePrice * Quantity);
                    ps.ProductName = selectedProduct.Name;
                    ps.SalePrice = ObjSale.SalePrice;
                    ps.Discount = 0;
                    ps.TotalSaleAmount = (ObjSale.SalePrice * Quantity);

                    decimal TaxAmount = 0;

                    ps.RemainingQuantity = warehse.RemainingQuantity;

                    ps.TotalSaleAmountWithTax = (selectedProduct.SalePrice * Quantity);//+ TaxAmount
                    ps.TaxAmount = TaxAmount;
                    ps.Profit = (ps.TotalSaleAmount - (ps.TotalPurchaseAmount));//+ TaxAmount
                    ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);//+ TaxAmount

                    ps.Description = "Sale Return";
                    ps.AddedBy = AddedBy;
                    ps.DateAdded = DateTime.Now;
                    ps.ModifiedBy = AddedBy;
                    ps.DateModied = DateTime.Now;
                    ps.InventoryTypeId = 4;
                    ps.WarehouseId = warehouses;
                    ps.IsFormal = false;
                    //   ps.ProductBatchId = db.ProductBatches.FirstOrDefault(i => i.BatchNumber == "Sale").Id;
                    ps.ReturnedQuantity = Quantity;
                    db.ProductStock.Add(ps);
                    db.SaveChanges();




                    sb.Append("Sumitted");
                    return Content(sb.ToString());


                }
                else
                {
                    sb.Append("Error : Not permitted  " + Quantity + " is greater than " + ObjSale.Quantity);
                    return Content(sb.ToString());

                }

            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());
        }

























        // GET: /Sale/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            Sale ObjSale = db.Sales.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                ViewBag.CustomerUserId = new SelectList(db.Users, "Id", "UserName", ObjSale.CustomerUserId);
                ViewBag.PaymentModeId = new SelectList(db.PaymentModes, "Id", "Name", ObjSale.PaymentModeId);
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ObjSale.ProductId);

            }

            return View(ObjSale);
        }

        // POST: /Sale/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(Sale ObjSale)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjSale).State = EntityState.Modified;
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


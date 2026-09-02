using ShopMate.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class PurchaseController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
        int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
        // GET: /Purchase/
        public ActionResult Index()
        {
            return View();
        }

        // GET Purchase/GetGrid?days=30  (defaults to last 30 days — pass days=0 for all records)
        public ActionResult GetGrid(int days = 30)
        {
            int warehouses = int.Parse(Env.GetUserInfo("WarehouseId"));
            var warehouse = db.Warehouses.ToArray();
            var user = db.Users.ToArray();

            var purchaseQuery = db.Purchases.Where(i => i.WarehouseId == warehouses);
            if (days > 0)
            {
                var cutoff = DateTime.Now.Date.AddDays(-days);
                purchaseQuery = purchaseQuery.Where(i => i.DateAdded >= cutoff);
            }

            var tak = purchaseQuery.OrderByDescending(i => i.DateAdded).ToArray();
            var userwarehouse = db.Users.FirstOrDefault(n => n.UserName == userId).WarehouseId;
            if (userId == "Zimhope")
            {
                var result = from c in tak
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Vendor_Id.UserName),
            Convert.ToString(c.Product_ProductId.Name),
            Convert.ToString(c.Quantity),
            Convert.ToString(c.ReturnedQuantity),
            Convert.ToString(c.UnitPrice),
            Convert.ToString(c.TotalAmount),
               Convert.ToString(c.TaxAmount),
                 Convert.ToString(c.TotalAmountWithTax),
            Convert.ToString(c.DateAdded),
           Convert.ToString(user.FirstOrDefault(i=>i.Id==c.AddedBy).UserName),
            Convert.ToString(warehouse.FirstOrDefault(i=>i.Id==c.WarehouseId).Name),
            Convert.ToString(c.InventoryTypeId),
             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = from c in tak.Where(n => n.WarehouseId == userwarehouse)
                             select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.Vendor_Id.UserName),
            Convert.ToString(c.Product_ProductId.Name),
            Convert.ToString(c.Quantity),
            Convert.ToString(c.ReturnedQuantity),
            Convert.ToString(c.UnitPrice),
            Convert.ToString(c.TotalAmount),
               Convert.ToString(c.TaxAmount),
               Convert.ToString(c.TotalAmountWithTax),
            Convert.ToString(c.DateAdded),
           Convert.ToString(user.FirstOrDefault(i=>i.Id==c.AddedBy).UserName),
            Convert.ToString(warehouse.FirstOrDefault(i=>i.Id==c.WarehouseId).Name),
            Convert.ToString(c.InventoryTypeId),
             };
                return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
            }
            // int warehouses = int.Parse(Env.GetUserInfo("WarehouseId"));
            // var warehouse = db.Warehouses.ToArray();
            // var user = db.Users.ToArray();
            // var tak = db.Purchases.Where(i => i.WarehouseId == warehouses).ToArray();
            // var result = from c in tak
            //              select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            // Convert.ToString(c.User_VendorUserId.UserName),
            // Convert.ToString(c.Product_ProductId.Name),
            // Convert.ToString(c.Quantity),
            // Convert.ToString(c.ReturnedQuantity),
            // Convert.ToString(c.UnitPrice),
            // Convert.ToString(c.TotalAmount),
            // Convert.ToString(c.DateAdded),
            //Convert.ToString(user.FirstOrDefault(i=>i.Id==c.AddedBy).UserName),
            // Convert.ToString(warehouse.FirstOrDefault(i=>i.Id==c.WarehouseId).Name),
            // Convert.ToString(c.InventoryTypeId),
            //  };
            // return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: /Purchase/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: /Purchase/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase ObjPurchase = db.Purchases.Find(id);
            if (ObjPurchase == null)
            {
                return HttpNotFound();
            }
            return View(ObjPurchase);
        }
        // GET: /Purchase/Create

        public ActionResult Create()
        {
            //var userwarehouse = db.Users.FirstOrDefault(n => n.UserName == userId).WarehouseId;
            //ViewBag.VendorUserId = new SelectList(db.Vendors, "Id", "UserName");
            //ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");

            //var tax = db.Taxs.ToArray();
            //ViewBag.TaxId = new SelectList(tax, "Id", "Name");
            var userwarehouse = db.Users.FirstOrDefault(n => n.UserName == userId).WarehouseId;
            if (userId == "Zimhope")
            {
                ViewBag.VendorUserId = new SelectList(db.Vendors, "Id", "FullName");
                ViewBag.ProductId = new SelectList(db.Products.Where(m => m.IsActive == true), "Id", "Name");
                ViewBag.BatchId = new SelectList(db.ProductBatches, "Id", "BatchNumber");
            }
            else
            {
                ViewBag.VendorUserId = new SelectList(db.Vendors.Where(i => i.WarehouseId == userwarehouse), "Id", "UserName");
                ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == userwarehouse && n.IsActive == true), "Id", "Name");
                ViewBag.BatchId = new SelectList(db.ProductBatches.Where(m => m.Product_ProductId.WarehouseId == userwarehouse), "Id", "BatchNumber");
            }
            return View();
        }

        // POST: /Purchase/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Purchase ObjPurchase, int VendorUserId, InvoiceItems[] productss, decimal SubTotal,decimal VatTotal, string Description = "")
        {
            string result = "Error! Purchase  Is Not Complete!";
            //Get the current claims principal
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;

            // Get the claims values
            int warehouse = Int16.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.Actor)
                               .Select(c => c.Value).SingleOrDefault());
            try
            {
                string VendorName = db.Vendors.FirstOrDefault(i => i.Id == VendorUserId).FullName;
                int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
                // int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));

                try
                {

                    Invoice inv = new Invoice();
                    inv.AddedBy = AddedBy;
                    inv.DateAdded = DateTime.Now;
                    inv.DateModied = DateTime.Now;
                    inv.IsBilled = false;
                    inv.IsPurchaseOrSale = "Purchase";
                    inv.ModifiedBy = AddedBy;
                    inv.UserId = VendorUserId;
                    inv.WarehouseId = warehouse;
                    inv.total = SubTotal+VatTotal;
                    inv.vat = VatTotal;
                    inv.subtotal = SubTotal;

                    db.Invoices.Add(inv);

                    db.SaveChanges(userId);

                    foreach (var item in productss)
                    {
                        var selectedProduct = db.Products.FirstOrDefault(i => i.Id == item.ProductId);
                        var ObjWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.ProductId && i.WarehouseId == warehouse);


                        var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == selectedProduct.TaxId);
                        // if (TaxName.Contains("IGST") || TaxName.Contains("Other"))
                        // {
                        //   selectedTax = db.Taxs.FirstOrDefault(i => i.Name == TaxName.Trim());
                        // }
                        //   Purchase ObjPurchase = new Models.Purchase();

                        ObjPurchase.ProductId = item.ProductId;
                        ObjPurchase.Quantity = item.Quantity;
                        // ObjPurchase.UnitPrice = selectedProduct.PurchasePrice;
                        ObjPurchase.UnitPrice = item.SalePrice;
                        ObjPurchase.TotalAmount = item.TotalAmount;
                        ObjPurchase.WarehouseId = warehouse;
                        ObjPurchase.InvoiceNumber = Description;
                        ObjPurchase.AddedBy = AddedBy;
                        ObjPurchase.TaxAmount = item.TaxAmount;
                        ObjPurchase.TotalAmountWithTax = item.TaxAmount+item.TotalAmount;
                        ObjPurchase.VendorUserId = VendorUserId;
                        ObjPurchase.DateAdded = DateTime.Now;


                        ObjPurchase.InventoryTypeId = 1;

                        db.Purchases.Add(ObjPurchase);
                        db.SaveChanges(userId);

                        //product begin here
                        string constring = System.Configuration.ConfigurationManager.ConnectionStrings["SIConnectionString"]
                   .ConnectionString;


                        string qury = "UPDATE Product SET PurchasePrice='" + item.SalePrice + "' WHERE 'Id'='" + selectedProduct.Id + "'";
                        using (SqlConnection con = new SqlConnection(constring))
                        {
                            using (SqlCommand cmd = new SqlCommand(qury, con))
                            {
                                con.Open();
                                cmd.ExecuteNonQuery();
                            }
                        }

                        //Product pr = new Product();
                        //pr.Id = selectedProduct.Id;
                        //pr.PurchasePrice = item.SalePrice;
                        //db.Entry(pr).State = EntityState.Modified;
                        //db.SaveChanges();
                        ProductStock ps = new ProductStock();
                        ps.ProductId = ObjPurchase.ProductId;
                        ps.Quantity = item.Quantity;
                        ps.PurchasePrice = item.SalePrice;

                        ps.ProductName = db.Products.FirstOrDefault(k => k.Id == ps.ProductId).Name;
                        ps.TotalPurchaseAmount = (item.SalePrice * item.Quantity);

                        ps.SalePrice = selectedProduct.SalePrice;

                        ps.Discount = selectedProduct.Discount;

                        decimal TaxAmount = 0;
                        decimal vatonreturn = 0;


                        ps.TotalSaleAmount = (item.SalePrice * item.Quantity) - vatonreturn;
                        ps.TotalSaleAmountWithTax =Convert.ToDecimal(ObjPurchase.TotalAmountWithTax);//(selectedProduct.SalePrice * item.Quantity);//+ TaxAmount
                        ps.TaxAmount = item.TaxAmount;
                        ps.Profit = (ps.TotalSaleAmount - ps.TotalPurchaseAmount) - vatonreturn;
                        ps.ProfitWithTax = (ps.TotalSaleAmountWithTax - ps.TotalPurchaseAmount);//+ TaxAmount
                                                                                                //  ps.ProductBatchId = db.ProductBatches.FirstOrDefault(i => i.BatchNumber == "Sale").Id;
                        ps.Description = "Purchase";
                        ps.AddedBy = ObjPurchase.AddedBy;
                        ps.DateAdded = DateTime.Now;
                        ps.ModifiedBy = ObjPurchase.AddedBy;
                        ps.DateModied = DateTime.Now;
                        ps.InventoryTypeId = 1;
                        ps.WarehouseId = warehouse;
                        db.ProductStock.Add(ps);
                        db.SaveChanges(userId);


                        //Get Ledger Account
                        int vendorLedger = 0;

                        var LedgerA = db.LedgerAccounts.FirstOrDefault(i => i.Name.Trim() == VendorName.Trim());
                        if (LedgerA != null)
                        {
                            vendorLedger = LedgerA.Id;
                        }
                        else
                        {
                            LedgerAccount la = new LedgerAccount();
                            la.Name = VendorName.Trim();
                            la.ParentId = 12;
                            la.AddedBy = AddedBy;
                            la.DateAdded = DateTime.Now;
                            db.LedgerAccounts.Add(la);
                            db.SaveChanges(userId);

                            vendorLedger = la.Id;
                        }
                        //end 

                        //transaction
                        Transaction tr = new Transaction();

                        tr.AddedBy = ObjPurchase.AddedBy;
                        //tr.DebitLedgerAccountId = 12;//Purchase ledger account
                        tr.DebitLedgerAccountId = vendorLedger;
                        tr.DebitAmount = (ObjPurchase.TotalAmount);//+ TaxAmount                        
                        tr.CreditLedgerAccountId = 13;
                        tr.CreditAmount = (ObjPurchase.TotalAmount);//+ TaxAmount
                        tr.DateAdded = DateTime.Now;
                        tr.Remarks = "Purchase, Purchase Account debit and " + VendorName + " account credit";
                        tr.Other = null;
                        tr.PurchaseOrSale = "Purchase";
                        tr.PurchaseIdOrSaleId = ObjPurchase.Id;
                        tr.WarehouseId = warehouse;
                        tr.IsFormal = true;
                        db.Transactions.Add(tr);

                        //end

                        db.SaveChanges(userId);


                        InvoiceItems Iitem = new InvoiceItems();

                        Iitem.ProductId = item.ProductId;
                        Iitem.Quantity = item.Quantity;
                        Iitem.TaxAmount = TaxAmount;
                        Iitem.AddedBy = ObjPurchase.AddedBy;
                        Iitem.DateAdded = DateTime.Now;
                        // Iitem.SalePrice = selectedProduct.PurchasePrice;
                        Iitem.SalePrice = item.SalePrice;
                        Iitem.TotalAmount = ObjPurchase.TotalAmount ;
                        Iitem.TotalAmountWithTax =(decimal)ObjPurchase.TotalAmountWithTax ;//+ TaxAmount
                        Iitem.TaxId = selectedTax.Id;
                        Iitem.PurchaseId = ObjPurchase.Id;
                        Iitem.SaleId = null;
                        Iitem.ProductStockId = 2;
                        Iitem.TransactionId = tr.Id;
                        Iitem.WarehouseId = warehouse;

                        Iitem.InvoiceId = inv.Id;
                        db.InvoiceItemss.Add(Iitem);

                        db.SaveChanges(userId);

                        ObjWarehouseStock.RemainingQuantity = ObjWarehouseStock.RemainingQuantity + ObjPurchase.Quantity;

                        db.Entry(ObjPurchase).State = EntityState.Modified;
                        db.SaveChanges(userId);



                        ProductStock ngonie = db.ProductStock.FirstOrDefault(k => k.Id == ps.Id);
                        ngonie.RemainingQuantity = ObjWarehouseStock.RemainingQuantity;
                        db.Entry(ngonie).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    result = "Success! Purchase Completed";
                    return Json(result, JsonRequestBehavior.AllowGet);


                }
                catch (Exception ex)
                {
                    Helper.WriteError(ex, ex.Message);
                    // retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
                }


            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                /// retVal.Add(new SaleReturn { msg = "error:" + ex.Message, value = 0 });
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase ObjPurchase = db.Purchases.Find(id);
            if (ObjPurchase == null)
            {
                return HttpNotFound();
            }
            ViewBag.VendorUserId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Vendor"), "Id", "UserName", ObjPurchase.VendorUserId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ObjPurchase.ProductId);

            return View(ObjPurchase);
        }

        // POST: /Purchase/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Purchase ObjPurchase)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjPurchase).State = EntityState.Modified;
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
        // GET: /Purchase/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase ObjPurchase = db.Purchases.Find(id);
            if (ObjPurchase == null)
            {
                return HttpNotFound();
            }
            return View(ObjPurchase);
        }

        // POST: /Purchase/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {


                Purchase ObjPurchase = db.Purchases.Find(id);

                InvoiceItems ObjInvoiceItems = db.InvoiceItemss.FirstOrDefault(i => i.PurchaseId == ObjPurchase.Id);

                Invoice ObjInvoice = db.Invoices.FirstOrDefault(i => i.Id == ObjInvoiceItems.InvoiceId);

                ProductStock ObjProductStock = db.ProductStock.FirstOrDefault(i => i.Id == ObjInvoiceItems.ProductStockId);

                Transaction ObjTransaction = db.Transactions.FirstOrDefault(i => i.Id == ObjInvoiceItems.TransactionId);


                if (ObjPurchase.InventoryTypeId == 1)
                {
                    var selectedProduct = db.Products.FirstOrDefault(i => i.Id == ObjPurchase.ProductId);

                    selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ObjPurchase.Quantity;
                    selectedProduct.RemainingAmount = selectedProduct.RemainingAmount - (ObjProductStock.TotalSaleAmountWithTax);

                    db.Entry(selectedProduct).State = EntityState.Modified;
                    db.SaveChanges(userId);
                }


                db.ProductStock.Remove(ObjProductStock);

                db.Transactions.Remove(ObjTransaction);

                db.Purchases.Remove(ObjPurchase);

                db.InvoiceItemss.Remove(ObjInvoiceItems);

                db.Invoices.Remove(ObjInvoice);

                db.SaveChanges(userId);

                try
                {
                    //if double antry of purchase or purchase retrun in transaction 
                    Transaction ObjTran2 = db.Transactions.FirstOrDefault(i => i.PurchaseOrSale == "Purchase" && i.PurchaseIdOrSaleId == id);
                    db.Transactions.Remove(ObjTran2);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                }

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
        // GET: /Purchase/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            Purchase ObjPurchase = db.Purchases.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                ViewBag.VendorUserId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Vendor"), "Id", "UserName", ObjPurchase.VendorUserId);
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ObjPurchase.ProductId);

            }

            return View(ObjPurchase);
        }

        // POST: /Purchase/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(Purchase ObjPurchase)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjPurchase).State = EntityState.Modified;
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

        [HttpGet]
        public ActionResult PurchaseReturn(int id)
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
        public ActionResult PurchaseReturn(decimal Quantity, int? id)
        {
            //algorithem
            ///if full quantity return than
            ///Purchase (Just change inventorytypeid)
            ///ProductStock (Just change inventorytypeid)
            ///[Transaction] (make reverse new entry) :: 
            ///
            ///if less quantiry retun than
            ///Purchase (update qty and amounts as per purchase to same entry)
            ///ProductStock (update qty and amounts as per purchase to same entry)
            ///[Transaction] (make reverse new entry with full remarks how much buy and how much return) ::
            ///InvoiceItem (update qty and amounts as per purchase to same entry)
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                Purchase ObjPurchase = db.Purchases.Find(id);
                if (ObjPurchase.InventoryTypeId == 1)
                {
                    //if (ObjPurchase.Quantity == Quantity)
                    //{
                    InvoiceItems ObjInvoiceItems = db.InvoiceItemss.FirstOrDefault(i => i.PurchaseId == ObjPurchase.Id);

                    ProductStock ObjProductStock = db.ProductStock.FirstOrDefault(i => i.Id == ObjInvoiceItems.ProductStockId);

                    Transaction ObjTransaction = db.Transactions.FirstOrDefault(i => i.Id == ObjInvoiceItems.TransactionId);
                    var selectedProduct = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == ObjPurchase.ProductId && i.WarehouseId == warehouse);
                    // var selectedProduct = db.Products.FirstOrDefault(i => i.Id == ObjPurchase.ProductId);
                    var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == selectedProduct.Product_ProductId.TaxId);



                    if (ObjPurchase.Quantity >= Quantity)
                    {
                        selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - Quantity;


                        db.Entry(selectedProduct).State = EntityState.Modified;
                        db.SaveChanges(userId);

                        ObjPurchase.ReturnedQuantity = ObjPurchase.ReturnedQuantity + Quantity;
                        ObjPurchase.Quantity = ObjPurchase.Quantity - Quantity;
                        ObjPurchase.TotalAmount = ObjPurchase.TotalAmount - (ObjPurchase.UnitPrice * Quantity);

                        db.Entry(ObjPurchase).State = EntityState.Modified;


                        ProductStock ps = new ProductStock();
                        ps.ProductId = ObjPurchase.ProductId;
                        ps.Quantity = Quantity;
                        ps.PurchasePrice = ObjPurchase.UnitPrice;

                        ps.TotalPurchaseAmount = (ObjPurchase.UnitPrice * Quantity);

                        ps.SalePrice = selectedProduct.Product_ProductId.SalePrice;

                        ps.Discount = 0;

                        decimal TaxAmount = 0;
                        decimal vatonreturn = 0;


                        ps.TotalSaleAmount = (ps.SalePrice * ps.Quantity);
                        ps.TotalSaleAmountWithTax = (ps.TotalSaleAmount);//+ TaxAmount
                        ps.TaxAmount = TaxAmount;
                        ps.Profit = 0;
                        ps.ProfitWithTax = (ps.TotalSaleAmountWithTax - ps.TotalPurchaseAmount);//+ TaxAmount
                        ps.ProductName = ObjPurchase.Product_ProductId.Name;
                        ps.Description = "Purchase Return";
                        ps.AddedBy = ObjPurchase.AddedBy;
                        ps.DateAdded = DateTime.Now;
                        ps.ModifiedBy = ObjPurchase.AddedBy;
                        ps.DateModied = DateTime.Now;
                        ps.InventoryTypeId = 3;
                        ps.WarehouseId = warehouse;
                        ps.ReturnedQuantity = Quantity;
                        ps.RemainingQuantity = selectedProduct.RemainingQuantity;
                        //   ps.ProductBatchId = db.ProductBatches.FirstOrDefault(i => i.BatchNumber == "Sale").Id;
                        db.ProductStock.Add(ps);

                        db.SaveChanges(userId);




                        //Transaction tr = new Transaction();
                        //tr.Remarks = "Purchase Return, Purchase Account credit and " + db.Users.FirstOrDefault(n => n.Id == ObjPurchase.VendorUserId).UserName + " account debit";
                        //tr.DebitLedgerAccountId = ObjTransaction.CreditLedgerAccountId;
                        //tr.CreditLedgerAccountId = ObjTransaction.DebitLedgerAccountId;
                        //tr.AddedBy = AddedBy;
                        //tr.DebitAmount = ObjTransaction.DebitAmount;
                        //tr.CreditAmount = ObjTransaction.CreditAmount;
                        //tr.DateAdded = DateTime.Now;
                        //tr.Other = "Retrun";
                        //tr.PurchaseOrSale = "Purchase";
                        //tr.PurchaseIdOrSaleId = ObjPurchase.Id;
                        //tr.WarehouseId = warehouse;
                        //tr.IsFormal = true;
                        //db.Transactions.Add(tr);

                        //db.SaveChanges(userId);

                        sb.Append("Sumitted");
                        return Content(sb.ToString());
                    }
                    else
                    {
                        sb.Append("Error : your product stock remaining quantity is (" + ObjPurchase.Quantity + ") low than your given return quantity.");
                        return Content(sb.ToString());
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


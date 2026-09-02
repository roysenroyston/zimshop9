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
using ShopMate.ModelDto;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class InvoiceController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        int warehouseId =int.Parse( Env.GetUserInfo("WarehouseId"));
        // GET: /Invoice/ for purchase
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DuePayments()
        {
            return View();
        }
        public ActionResult GetDuePayments()
        {
            var tak = db.Invoices.Where(i => i.IsPurchaseOrSale == "Sale" && i.CustomerId > 0 && i.balance > 0 && i.Duedate < DateTime.Now).ToArray();
            //var tak2 = db.InformalInvoices.Where(i => i.IsPurchaseOrSale == "Sale" && i.CustomerId > 0 && i.balance > 0 && i.Duedate < DateTime.Now).ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(db.Users.FirstOrDefault(i => i.Id == (c.CustomerId)).UserName),
            Convert.ToString(c.orderNumber),
            Convert.ToString(c.total),
            Convert.ToString(c.payment),
            Convert.ToString(c.balance),
            Convert.ToString(c.DateAdded),
            Convert.ToString(c.Duedate),
             Convert.ToString(db.Warehouses.FirstOrDefault(i => i.Id == (c.WarehouseId)).Name),
             };
     
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        //new invoice
        //new invoice
        public ActionResult NewInvoice()
        {
            ViewBag.CustomerUserId = new SelectList(db.Customers.Where(i=> i.WarehouseId == warehouseId), "Id", "UserName");
            ViewBag.CustomerVATReg = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Customer" && i.WarehouseId == warehouseId), "Id", "CustomerVatReg");
            ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(m=> m.Id== warehouseId), "Id", "Name");
            ViewBag.salesrepId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "SalesRep" && i.WarehouseId == warehouseId), "Id", "UserName");
            ViewBag.ProductId = new SelectList(db.Products.Where(m=> m.WarehouseId == warehouseId), "Id", "Name");
            ViewBag.PaymentMethods = new SelectList(db.InvoicePaymentMethods, "id", "Name");

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult NewInvoice(int customerid, int? orderNo, string vatReg, int ProjectNumber, decimal subtotal, decimal vat, decimal total, decimal payment, decimal balance, int wareId, int PaymentMethodId, InvoiceMaterials[] invoicemat, int? salesrepid, decimal? totalDiscount)
        {
            DateTime today = DateTime.Now;

            var selectedSaleOrder = db.SaleOrders.FirstOrDefault(i => i.Id == orderNo);
            bool IsFormalInvoice = true;
            //if(db.Users.FirstOrDefault(i => i.Id == (customerid)).vatNumber == null || db.Users.FirstOrDefault(i => i.Id == (customerid)).vatNumber == "")
            //{
            //    IsFormalInvoice = false;

            //}
            int invoiceId = 0;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string returval = "Invoice failed to process ";
            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            try
            {

                if (ModelState.IsValid)
                {
                   
                     Invoice ObjInvoice = new Invoice();
                    InformalInvoice ObjInformalInvoices = new InformalInvoice();
                    //if (IsFormalInvoice == true)
                    //{
                        ObjInvoice.UserId = customerid;
                    ObjInvoice.orderNumber = orderNo;
                    // ObjInvoice.InvoiceNo = jobNo;
                    ObjInvoice.IsBilled = false;
                    ObjInvoice.CustomerVatReg = vatReg;
                    ObjInvoice.ProjectNumber = ProjectNumber;
                    ObjInvoice.subtotal = subtotal;
                    ObjInvoice.vat = vat;
                    ObjInvoice.total = total;
                    ObjInvoice.payment = payment;
                    ObjInvoice.balance = balance;
                    ObjInvoice.totalDiscount = totalDiscount;
                    ObjInvoice.IsPurchaseOrSale = "Sale";
                    ObjInvoice.WarehouseId = wareId;
                    ObjInvoice.DateAdded = DateTime.Now;
                    ObjInvoice.DateModied = DateTime.Now;
                    ObjInvoice.AddedBy = AddedBy;
                    ObjInvoice.InvoiceNo = ObjInvoice.Id;
                    ObjInvoice.salesrepId = salesrepid;
                    ObjInvoice.CustomerId = customerid;
                    ObjInvoice.InvoicePaymentMethodId = PaymentMethodId;
                        //ObjInvoice.Duedate = DateTime.Now + db.InvoicePaymentMethods.FirstOrDefault(i => i.Id == (ObjInvoice.InvoicePaymentMethodId)).DueIn);
                        var duein = db.InvoicePaymentMethods.FirstOrDefault(i => i.Id == (ObjInvoice.InvoicePaymentMethodId)).DueIn;
                        DateTime answer = today.AddDays(duein);
                        ObjInvoice.Duedate = answer;

                        db.Invoices.Add(ObjInvoice);
                    //}
                    //else
                    //{
                    //    ObjInformalInvoices.UserId = customerid;
                    //    ObjInformalInvoices.orderNumber = orderNo;
                    //    // ObjInvoice.InvoiceNo = jobNo;
                    //    ObjInformalInvoices.IsBilled = false;
                    //    ObjInformalInvoices.CustomerVatReg = vatReg;
                    //    ObjInformalInvoices.ProjectNumber = ProjectNumber;
                    //    ObjInformalInvoices.subtotal = subtotal;
                    //    ObjInformalInvoices.vat = vat;
                    //    ObjInformalInvoices.total = total;
                    //    ObjInformalInvoices.payment = payment;
                    //    ObjInformalInvoices.balance = balance;

                    //    ObjInformalInvoices.IsPurchaseOrSale = "Sale";
                    //    ObjInformalInvoices.WarehouseId = wareId;
                    //    ObjInformalInvoices.DateAdded = DateTime.Now;
                    //    ObjInformalInvoices.DateModied = DateTime.Now;
                    //    ObjInformalInvoices.AddedBy = AddedBy;
                    //    ObjInformalInvoices.InvoiceNo = ObjInvoice.Id;
                    //    ObjInformalInvoices.salesrepId = salesrepid;
                    //    ObjInformalInvoices.InvoicePaymentMethodId = PaymentMethodId;
                    //    ObjInformalInvoices.CustomerId = customerid;
                    //    var duein = db.InvoicePaymentMethods.FirstOrDefault(i => i.Id == (ObjInformalInvoices.InvoicePaymentMethodId)).DueIn;
                    //    DateTime answer = today.AddDays(duein);
                    //    ObjInformalInvoices.Duedate = answer;
                    //    db.InformalInvoices.Add(ObjInformalInvoices);
                    //}
                    db.SaveChanges(userId);
                    Sale ObjSale = new Models.Sale();

                    foreach (var inmat in invoicemat)
                    {
                        var selectedProduct = db.Products.FirstOrDefault(i => i.Id == inmat.ProductId);                     

                   
                        var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == selectedProduct.TaxId);
                        // Add to invoice items
                    

                        ObjSale.ProductId = inmat.ProductId;
                        ObjSale.Quantity = inmat.quantity;
                        ObjSale.SalePrice = selectedProduct.SalePrice;
                        ObjSale.TotalAmount = (selectedProduct.SalePrice * ObjSale.Quantity);
                        ObjSale.WarehouseId = wareId;
                        ObjSale.AddedBy = AddedBy;
                        ObjSale.CustomerUserId = customerid;
                        ObjSale.DateAdded = DateTime.Now;
                        ObjSale.DateModied = DateTime.Now;
                        ObjSale.ModifiedBy = AddedBy;
                        ObjSale.PaidAmount = (selectedProduct.SalePrice * ObjSale.Quantity);
                        ObjSale.PaymentModeId = 5;// Norlin remove hard codin  come back here
                        ObjSale.InventoryTypeId = 2; // why is inventory InventoryTypeId hard coded
                        ObjSale.discount = inmat.discount;
                        ObjSale.isFormalSale = IsFormalInvoice;
                     

                        // ObjSale.bond = bond;
                        //    ObjSale.rtgs = swipe;
                        //   ObjSale.ecocash = ecocash;

                        db.Sales.Add(ObjSale);
                        db.SaveChanges(userId);
                        Purchase ObjPurchase = new Purchase();

                        //ProductStock begin
                        ProductStock ps = new ProductStock();
                        ps.ProductId = ObjSale.ProductId;
                        ps.Quantity = ObjSale.Quantity;

                        ps.PurchasePrice = selectedProduct.PurchasePrice;
                        ps.ddiscount = inmat.discount;
                        ps.TotalPurchaseAmount = (selectedProduct.PurchasePrice * ObjSale.Quantity);

                        ps.SalePrice = selectedProduct.SalePrice;
                        ps.Discount = inmat.discount;
                        ps.TotalSaleAmount = (selectedProduct.SalePrice * ObjSale.Quantity);

                        decimal TaxAmount = 0;
                        //if (selectedTax.Other == "GST")
                        //{
                        //    decimal taxSplit = selectedTax.TaxRate / 2;
                        //    ps.CGST = selectedTax.Id;
                        //    ps.CGST_Rate = taxSplit;
                        //    ps.SGST = selectedTax.Id;
                        //    ps.SGST_Rate = taxSplit;

                        //    TaxAmount = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                        //}
                        //else if (selectedTax.Other == "IGST")
                        //{
                        //    ps.IGST = selectedTax.Id;
                        //    ps.IGST_Rate = selectedTax.TaxRate;

                        //    TaxAmount = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                        //}
                        //else if (selectedTax.Other == "Other")
                        //{
                        //    ps.TaxId = selectedTax.Id;
                        //    ps.OtherTaxValue = selectedTax.TaxRate;
                        //    TaxAmount = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                        //}


                        ps.TotalSaleAmountWithTax = (selectedProduct.SalePrice * ObjSale.Quantity);//+ TaxAmount
                        ps.TaxAmount = TaxAmount;
                        ps.Profit = (ps.TotalSaleAmount - (ps.TotalPurchaseAmount));//+ TaxAmount
                        ps.ProfitWithTax = (ps.TotalSaleAmount - ps.TotalPurchaseAmount);//+ TaxAmount

                        ps.Description = "Invoice Sale";
                        ps.AddedBy = AddedBy;
                        ps.DateAdded = DateTime.Now;
                        ps.ModifiedBy = AddedBy;
                        ps.DateModied = DateTime.Now;
                        ps.InventoryTypeId = 2;
                        ps.WarehouseId = wareId;
                        if (IsFormalInvoice)
                        {
                            ps.IsFormal = true;
                        }
                        else
                        {
                            ps.IsFormal = false;
                        }
               //         ps.ProductBatchId = db.ProductBatches.FirstOrDefault(i => i.BatchNumber == "Sale").Id;
                        db.ProductStock.Add(ps);
                        db.SaveChanges(userId);

                        //end
                       
                        InvoiceMaterials mat = new InvoiceMaterials();
                        mat.ProductId = inmat.ProductId;
                        mat.description = inmat.description;
                        mat.quantity = inmat.quantity;
                        mat.rate = inmat.rate;
                        mat.vat = inmat.vat;
                        mat.discount = inmat.discount;
                        if (IsFormalInvoice)
                        {
                            mat.InvoiceId = ObjInvoice.Id;
                        }
                        else
                        {
                            mat.InformalInvoiceId = ObjInformalInvoices.Id;
                        }

                        db.InvoiceMaterial.Add(mat);
                        db.SaveChanges(userId);


                        //Get Ledger Account
                        int vendorLedger = 0;
                    string CustomerName = db.Users.FirstOrDefault(i => i.Id == customerid).UserName;
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
                        la.AddedBy = AddedBy;
                        la.DateAdded = DateTime.Now;
                        db.LedgerAccounts.Add(la);
                        db.SaveChanges();

                        vendorLedger = la.Id;
                    }
                    //end 

                    //transaction
                    Transaction tr = new Transaction();
                    tr.AddedBy = AddedBy;
                    tr.DebitLedgerAccountId = vendorLedger;
                    tr.DebitAmount = total;
                    //tr.CreditLedgerAccountId = 11;
                    tr.CreditLedgerAccountId = db.LedgerAccounts.FirstOrDefault(i => i.Name == ("Sale")).Id;
                    tr.CreditAmount = payment;
                    tr.DateAdded = DateTime.Now;
                    tr.Remarks = "Sale, Sale Account credit and " + CustomerName + " account debit";
                    tr.Other = null;
                    tr.PurchaseOrSale = "Sale";
                    if (IsFormalInvoice == true)
                    {
                        tr.IsFormal = true;
                        tr.PurchaseIdOrSaleId = ObjInvoice.Id;
                    }
                    else
                    {
                        tr.IsFormal = false;
                        tr.PurchaseIdOrSaleId = ObjInformalInvoices.Id;
                    }
                    tr.PurchaseIdOrSaleId = ObjInvoice.Id;
                    tr.WarehouseId = wareId;
                    db.Transactions.Add(tr);
                    //end

                    db.SaveChanges(userId);

                        InvoiceItems Iitem = new InvoiceItems();

                        Iitem.ProductId = ObjSale.ProductId;
                        Iitem.Quantity = ObjSale.Quantity;
                        Iitem.TaxAmount = TaxAmount;
                        Iitem.AddedBy = AddedBy;
                        Iitem.DateAdded = DateTime.Now;
                        Iitem.SalePrice = selectedProduct.SalePrice;
                        Iitem.TotalAmount = ps.TotalSaleAmount;
                        Iitem.TotalAmountWithTax = ps.TotalSaleAmountWithTax;
                        Iitem.TaxId = selectedTax.Id;
                        Iitem.PurchaseId = null;
                        Iitem.SaleId = ObjSale.Id;
                        Iitem.ProductStockId = ps.Id;
                        Iitem.TransactionId = tr.Id;
                        Iitem.WarehouseId = wareId;
                        Iitem.discount = ObjSale.discount;
                        if (IsFormalInvoice)
                        {
                        Iitem.InvoiceId = ObjInvoice.Id;
                        
                        }
                        else
                        {
                            Iitem.InformalInvoiceId = ObjInformalInvoices.Id;
                        }
                        db.InvoiceItemss.Add(Iitem);
                        db.SaveChanges(userId);
                        if (orderNo.HasValue && orderNo.Value > 0)
                    {
                        
                        selectedSaleOrder.IsProcessed = true;
                            selectedSaleOrder.DateModified = DateTime.Now;
                            db.Entry(selectedSaleOrder).State = EntityState.Modified;
                        db.SaveChanges(userId);
                     }

                    selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - ObjSale.Quantity;
                    selectedProduct.RemainingAmount = selectedProduct.RemainingAmount - ps.TotalSaleAmountWithTax;

                    var saleUpdate = db.Sales.FirstOrDefault(i => i.Id == ObjSale.Id);
                    saleUpdate.TotalAmountWithTax = ps.TotalSaleAmountWithTax;
                    db.Entry(saleUpdate).State = EntityState.Modified;

                    db.Entry(selectedProduct).State = EntityState.Modified;
                    db.SaveChanges(userId);
                }
                    if (IsFormalInvoice)
                    {
                        invoiceId = ObjInvoice.Id;
                    }
                    else
                    {
                        invoiceId = ObjInformalInvoices.Id;
                    }
                    List<SaleReturn> retVal = new List<SaleReturn>();
                    sb.Append("Submitted");
                    //returval = "Submitted";
                    retVal.Add(new SaleReturn { msg = "Submitted", value = invoiceId });
                    return Json(retVal, JsonRequestBehavior.AllowGet);
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
        public class SaleReturn
        {
            public string msg { get; set; }
            public int value { get; set; }
            public bool isformal { get; set; }

        }
        // GET Invoice/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.Invoices.Where(i => i.IsPurchaseOrSale == "Purchase").ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.IsBilled),
            Convert.ToString(c.UserId),
            Convert.ToString(c.IsPurchaseOrSale),
            Convert.ToString(c.AddedBy),
            Convert.ToString(c.DateAdded),
            Convert.ToString(c.DateModied),
            Convert.ToString(c.ModifiedBy),
             Convert.ToString(c.WarehouseId),
             

             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Sale()
        {
            return View();
        }
        // GET Invoice/GetGridSale?days=30  (defaults to last 30 days — pass days=0 for all records)
        public ActionResult GetGridSale(int days = 30)
        {
            var query = db.InformalInvoices.Where(i => i.IsPurchaseOrSale == "Sale" && i.WarehouseId == warehouseId);

            if (days > 0)
            {
                var cutoff = DateTime.Now.Date.AddDays(-days);
                query = query.Where(i => i.DateAdded >= cutoff);
            }

            var tak = query.OrderByDescending(i => i.DateAdded).ToArray();
            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            //Convert.ToString(db.Customers.FirstOrDefault(i => i.Id == (c.CustomerId)).UserName),
              Convert.ToString(c.IsBilled),
            Convert.ToString(c.InvoiceNo),
            Convert.ToString(c.total),
          db.Sales.FirstOrDefault(h => h.recieptNumber == c.InvoiceNo)?.zimraReceiptNo.ToString() ?? db.Sales.FirstOrDefault(h => h.InvoiceId == c.InvoiceNo)?.zimraReceiptNo.ToString()?? "NULL",
            //Convert.ToString(c.balance),
            Convert.ToString(c.DateAdded),
            //Convert.ToString(c.Duedate),
             Convert.ToString(db.Warehouses.FirstOrDefault(i => i.Id == (c.WarehouseId)).Name),
               Convert.ToString(c.CustomerVatReg),
                Convert.ToString(c.ErrorMessage),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
           
        }
        // GET: /Invoice/ModelBindIndex
        public ActionResult ModelBindIndex()
        {
            return View();
        }
        // GET: /Invoice/Details/5
        public ActionResult GetDetails(int? id)
        {
            //List<Invoice> lstInvoice = new List<Invoice>();
            Invoice itemInvoice = new Invoice();
            decimal? TotalBalance = 0;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ObjInvoice = db.Invoices.Where(i => i.CustomerId == (id) && i.balance > 0).ToArray();
            if (ObjInvoice == null)
            {
                var ObjInvoice2 = db.Invoices.Where(i => i.CustomerId == (id) && i.balance > 0).ToArray();
                if (ObjInvoice2 == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    foreach (var item in ObjInvoice2)
                    {
                        //itemInvoice.Id = item.Id;
                        //itemInvoice.balance=item.balance;
                        //lstInvoice.Add(itemInvoice);
                        TotalBalance += item.balance;


                    }
                    return Json(new { invoiceBalance = TotalBalance }, JsonRequestBehavior.AllowGet);
                }
            }
            foreach (var item in ObjInvoice)
            {
                //itemInvoice.Id = item.Id;
                //itemInvoice.balance = item.balance;
                //lstInvoice.Add(itemInvoice);
                TotalBalance += item.balance;
            }
            return Json(new { invoiceBalance = TotalBalance }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice ObjInvoice = db.Invoices.Find(id);
            if (ObjInvoice == null)
            {
                return HttpNotFound();
            }
            return View(ObjInvoice);
        }
        // GET: /Invoice/Create
        public ActionResult Create()
        {
             
             return View();
        }

        // POST: /Invoice/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Invoice ObjInvoice )
        { 
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                if (ModelState.IsValid)
                { 
                    

                    db.Invoices.Add(ObjInvoice);
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
        // GET: /Invoice/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice ObjInvoice = db.Invoices.Find(id);
            if (ObjInvoice == null)
            {
                return HttpNotFound();
            }
            
            return View(ObjInvoice);
        }

        // POST: /Invoice/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Invoice ObjInvoice )
        { 
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                if (ModelState.IsValid)
                { 
                    

                    db.Entry(ObjInvoice).State = EntityState.Modified;
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
        // GET: /Invoice/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice ObjInvoice = db.Invoices.Find(id);
            if (ObjInvoice == null)
            {
                return HttpNotFound();
            }
            return View(ObjInvoice);
        }

        // POST: /Invoice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        { 
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                  
                    Invoice ObjInvoice = db.Invoices.Find(id);
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
        // GET: /Invoice/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        { 
            Invoice ObjInvoice = db.Invoices.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                
            }
            
            return View(ObjInvoice);
        }

        // POST: /Invoice/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(Invoice ObjInvoice )
        {  
            System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
            try
            {
                if (ModelState.IsValid)
                { 
                    

                    db.Entry(ObjInvoice).State = EntityState.Modified;
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

        public ActionResult print(int? id)
        {
            try { 
            //return HttpNotFound();
            if(id == null)
            {              
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            bool IsFormalInvoice = false;
            InvoiceDto inv = new InvoiceDto();
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoice = db.Invoices.FirstOrDefault(i => i.Id == id && i.WarehouseId == warehouse);
            var Informalinvoice = db.InformalInvoices.FirstOrDefault(i => i.Id == id && i.WarehouseId == warehouse);
             
            int? CustomerId;
            //string CustomerBranchName;
                Customers user = new Customers();
                int customerId = Informalinvoice.CustomerId ?? 17;
                user = db.Customers.FirstOrDefault(i => i.Id == customerId);
                CustomerId = db.InformalInvoices.FirstOrDefault(i => i.Id == id).CustomerId;
       
           
            var invoiceitem = db.Sales.Where(i => i.recieptNumber == Informalinvoice.InvoiceNo).ToArray()?? db.Sales.Where(i => i.InvoiceId == Informalinvoice.InvoiceNo).ToArray();
            var serviceitem = db.InvoiceMaterial.Where(i => i.InvoiceId == id).ToArray();
                var myItem = db.Sales.FirstOrDefault(h => h.recieptNumber == Informalinvoice.InvoiceNo)?? db.Sales.FirstOrDefault(h => h.InvoiceId == Informalinvoice.InvoiceNo);
                var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            var setting = db.Settings.Where(i => i.sGroup == "Invoice").ToArray();
            var tax = db.Taxs.ToArray();
               
            inv.InvoiceId = id;
            if (IsFormalInvoice) {
                inv.InvoiceDate = invoice.DateAdded.Value;
                inv.Type = invoice.IsPurchaseOrSale;
                inv.Duedate =invoice.Duedate;
                //inv.PaymentTerms = db.InvoicePaymentMethods.FirstOrDefault(i => i.Id == invoice.InvoicePaymentMethodId).Name;
                db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
                }
            else
            {
                inv.InvoiceDate = Informalinvoice.DateAdded.Value;
                inv.Type = Informalinvoice.IsPurchaseOrSale;
                    inv.Duedate = Informalinvoice.Duedate;
                    serviceitem = db.InvoiceMaterial.Where(i => i.InformalInvoiceId == id).ToArray();
                invoiceitem = db.Sales.Where(h => h.recieptNumber == Informalinvoice.InvoiceNo).ToArray();
            }
                var me1 = db.PaymentModes.Where(c => c.Id == myItem.PaymentModeId).FirstOrDefault();
           
            inv.InvoiceFooterText = invoiceFormat.FooterInfo;

                inv.baseLogo = invoiceFormat.baseLogo;

                inv.Logo = Env.GetSiteRoot() + "/Uploads/" + invoiceFormat.Logo;
           // inv.CurrencySymbol = setting.FirstOrDefault(i => i.sKey == "CurrencySymbol").sValue;
                inv.CurrencySymbol = db.Currencies.FirstOrDefault(j=>j.Name==me1.Name && j.WarehouseId== warehouse).CurrencySymbol;
                inv.ToName = user.BuyerRegisterName;
               // inv.ToName = user.UserName;
            inv.ToInfo = user.HouseNo + " " + user.Street + " " + user.City;
            inv.CompanyAddress = invoiceFormat.AddressInfo;
            inv.CompanyContact = invoiceFormat.OtherInfo;
            inv.CompanyName = invoiceFormat.CompanyName;
            inv.BranchName = user.BuyerTradeName;
            inv.CompanyvatNo = invoiceFormat.VatNumber;
            inv.vatNo = user.VATNumber;
                inv.tinNo = invoiceFormat.taxPayerTIN;
                inv.Cstomer = myItem.CustomerName;
               // inv.custinNo = user.BuyerTIN;

             //   inv.BranchName = CustomerBranchName;
                inv.CompanyvatNo = invoiceFormat.VatNumber;
                inv.BP = invoiceFormat.BPNumber;
                //inv.vatNo = user.VATNumber;

                inv.verificationCode = myItem.VerificationCode;
                inv.Zimraemail = myItem.qrUrl;
                inv.deviceSerialNo = myItem.deviceSerialNo;
                inv.fiscalDayNumber = myItem.fiscalDayNumber;
                inv.deviceID = myItem.deviceID;
                inv.qrCode = myItem.qrCode;
                inv.receiptId = myItem.zimraReceiptNo;
                inv.email = db.Warehouses.FirstOrDefault(j => j.Id == warehouse).Email;


                List<InvoiceItemsDto> listItem = new List<InvoiceItemsDto>();

            foreach (var item in invoiceitem)
            {
                InvoiceItemsDto li = new InvoiceItemsDto();
                li.Price = item.SalePrice;
                li.ProcuctName = item.Product_ProductId.Name;
                li.Quantity = item.Quantity;
                li.Tax = (decimal)item.TotalAmountWithTax;
                

                li.TaxInfo = tax.FirstOrDefault(i => i.Id == 2).Name;

                li.SubTotal = item.TotalAmount;
                listItem.Add(li);
            }
            List<servicesDto> serviceItems = new List<servicesDto>();

            foreach (var items in serviceitem)
            {
                servicesDto lii = new servicesDto();
                lii.Code = items.Product_ProductId.BarCode;
                lii.productn = items.Product_ProductId.Name;
                lii.description = items.description;
                lii.quantity = items.quantity;
                lii.rates = items.rate;
                lii.total = (items.rate * items.quantity);
                    lii.total = decimal.Round(lii.total, 2);
                    lii.vat = items.vat;
                    lii.discount = items.discount;
                    serviceItems.Add(lii);


            }
            if (IsFormalInvoice)
            {
                inv.SubTotal =  invoice.subtotal;
                inv.Tax = invoice.vat;
                    //inv.TotalAmount = invoiceitem.Sum(i => i.TotalAmountWithTax);
                    inv.totalDiscount = invoice.totalDiscount;
                    inv.TotalAmount = (inv.Tax + inv.SubTotal) - inv.totalDiscount;
                    inv.payment = invoice.payment;
                inv.balance = invoice.balance;
            }
            else
            {
                    //inv.SubTotal = invoiceitem.Sum(i => i.TotalAmount) + Informalinvoice.subtotal;
                    inv.SubTotal =  Informalinvoice.subtotal;
                    inv.Tax = Informalinvoice.vat;
                    //inv.TotalAmount = invoiceitem.Sum(i => i.TotalAmountWithTax);
                    inv.TotalAmount = inv.SubTotal + inv.Tax;
                    inv.payment = Informalinvoice.payment;
                inv.balance = Informalinvoice.balance;
            }
            inv.invoiceItem = listItem;
            inv.services = serviceItems;       
            inv.TaxInfo = "";      

            return View(inv);
            }
            catch(Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return HttpNotFound(ex.Message);
            }
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


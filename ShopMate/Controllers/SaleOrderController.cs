using Newtonsoft.Json;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;
using static ShopMate.Controllers.posController;

namespace ShopMate.Controllers
{
    public class SaleOrderController : BaseController
    {
        string userId = Env.GetUserInfo("name");
        // GET: SaleOrder
        public ActionResult Index()
        {
            return View();
        }

        // GET SaleOrder/GetGrid
        public ActionResult GetGrid()
        {
            var tak = db.SaleOrders.ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(db.Users.FirstOrDefault(i => i.Id == (c.CustomerId)).UserName),
            Convert.ToString(db.Warehouses.FirstOrDefault(i => i.Id == (c.WarehouseId)).Name),
            Convert.ToString(c.DateAdded),
            //Convert.ToString(c.IssueDate),
            //Convert.ToString(c.Product),
            //Convert.ToString(c.IssueDate),
            //Convert.ToString(c.Product),

            };

            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetProduct()
        {


            var tak = db.Products.OrderBy(i => i.Name).ToArray();
            var resul = new string[] { };

            var result = from c in tak
                         select new string[] {
            Convert.ToString(c.Name.Replace("'","")),
            Convert.ToString(c.SalePrice),
            Convert.ToString(c.Id) ,
            Convert.ToString(c.ProductImage) ,
            Convert.ToString(c.SalePrice),
            Convert.ToString(db.Taxs.FirstOrDefault(i=>i.Id== c.TaxId).TaxRate),
             Convert.ToString(c.BarCode),
             Convert.ToString(c.RemainingQuantity),

             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);

        }
        // GET: /SaleOrder/ModelBindIndex
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
            SaleOrder ObjSaleOrder = db.SaleOrders.Find(id);
            if (ObjSaleOrder == null)
            {
                return HttpNotFound();
            }
            return View(ObjSaleOrder);
        }
        public ActionResult GetDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ObjSaleOrder =  db.SaleOrders.Where(i => i.CustomerId == (id) && i.IsProcessed == false).ToArray();
            if (ObjSaleOrder == null)
            {
                return HttpNotFound();
            }
            return Json(new { salesorders = ObjSaleOrder}, JsonRequestBehavior.AllowGet);
        }

    public ActionResult GetSaleOrderItems(int? id)
        {
            try { 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ObjSaleOrdertems = db.SaleOrderItems.Where(i => i.SaleOrderId == (id) ).ToArray();
                List<OrderSaleItem> lstOrderSaleItem = new List<OrderSaleItem>();
                foreach (var item in ObjSaleOrdertems)
                {
                    OrderSaleItem orderSaleItem = new OrderSaleItem();
                    orderSaleItem.Name = db.Products.FirstOrDefault(i => i.Id == (item.ProductId)).Name;
                    orderSaleItem.price = item.SalePrice;
                    orderSaleItem.qty = item.Quantity;
                    orderSaleItem.productId = item.ProductId;
                    orderSaleItem.taxId = item.TaxId;
                    orderSaleItem.totalAmount = item.TotalAmount;
                    orderSaleItem.taxAmount = item.TaxAmount;
                    orderSaleItem.totalAmountWithTax = item.TotalAmountWithTax;
                    orderSaleItem.saleOrderId = item.SaleOrderId;
                    orderSaleItem.Description = db.Products.FirstOrDefault(i=>i.Id == item.ProductId).ProductDescription;
                    orderSaleItem.remainingquantity = db.Products.FirstOrDefault(i => i.Id == item.ProductId).RemainingQuantity;
                    lstOrderSaleItem.Add(orderSaleItem);
                }
                if (ObjSaleOrdertems == null)
                {   
                    return HttpNotFound();
                }
                var result = JsonConvert.SerializeObject(lstOrderSaleItem, Formatting.Indented,
                               new JsonSerializerSettings
                               {
                                   ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                               });
              
                return Json(result, JsonRequestBehavior.AllowGet);
                //return Json(new { saleorderItems = ObjSaleOrdertems }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return HttpNotFound(ex.Message);
            }
        }

        // GET: SaleOrder/Create
        public ActionResult Create()
        {

            //var customerUser = db.Users.Where(i => i.Role_RoleId.RoleName == "Customer");
            var customerUser = db.Users.Where(i => i.Role_RoleId.Id >0);
            ViewBag.CustomerUserId = new SelectList(customerUser, "Id", "UserName", customerUser.FirstOrDefault().Id);
          
            ViewBag.UserId = new SelectList(customerUser, "Id", "UserName");
            var paye = db.Currencies;
            //var paye = db.PaymentModes.Where(i => i.Id == 1);  

            ViewBag.PaymentModes = new SelectList(db.Currencies, "id", "Name");
            //ViewBag.PaymentModeId = new SelectList(paye, "Id", "Name",paye.FirstOrDefault().Sale_PaymentModeIds);

            StringBuilder sbMoreTax = new StringBuilder();
            var tax = db.Taxs.Where(i => i.Other == "Tax").ToArray();
            foreach (var item in tax)
            {
                sbMoreTax.Append("<option value=\"" + item.Name + "\">" + item.Name + "</option>");
            }

            ViewBag.moreTax = sbMoreTax.ToString();

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        //public ActionResult Create(Purchase ObjPurchase, int CustomerId, InvoiceItems[] productss, string Description = "")
        //public ActionResult Create(SaleOrder ObjSaleOrder, SaleOrderItem[] SaleOrderItems, int wareId)
        //public ActionResult Create(SaleOrder ObjSaleOrder, int CustomerId, SaleOrderItem[] SaleOrderItems, string Description = "")
        public ActionResult Create(List<Cart> products, int CustomerUserId, string TaxName)
        {
            
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string result = "Error! Sale Order Is Not Complete!";
           //try here norlin
                string CustomerName = db.Users.FirstOrDefault(i => i.Id == CustomerUserId).UserName;
                int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
           
            try
            {

                SaleOrder ObjSaleOrder = new SaleOrder();
                ObjSaleOrder.AddedBy = AddedBy;
                ObjSaleOrder.DateAdded = DateTime.Now;
                ObjSaleOrder.DateModified = DateTime.Now;
                ObjSaleOrder.ModifiedBy = AddedBy;
                ObjSaleOrder.CustomerId = CustomerUserId;
                //ObjSaleOrder.User_CustomerId = CustomerUserId;
                ObjSaleOrder.WarehouseId = warehouse;
                ObjSaleOrder.IsProcessed = false;
                db.SaleOrders.Add(ObjSaleOrder);
                db.SaveChanges(userId);

                
                foreach (var item in products)
                    {
                    var selectedProduct = db.Products.FirstOrDefault(i => i.Id == item.product);
                    var selectedTax = db.Taxs.FirstOrDefault(i => i.Id == selectedProduct.TaxId);
                    if (TaxName.Contains("IGST") || TaxName.Contains("Other"))
                    {
                        selectedTax = db.Taxs.FirstOrDefault(i => i.Name == TaxName.Trim());
                    }

                    //ProductStock begin
                    ProductStock ps = new ProductStock();
                    ps.ProductId = item.product;
                    ps.Quantity = item.qty;

                    ps.PurchasePrice = selectedProduct.PurchasePrice;

                    ps.TotalPurchaseAmount = (selectedProduct.PurchasePrice * item.qty);

                    ps.SalePrice = selectedProduct.SalePrice;
                    ps.Discount = selectedProduct.Discount;
                    ps.TotalSaleAmount = (selectedProduct.SalePrice * item.qty);

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
                    ps.TaxId = selectedTax.Id;
                    //    ps.OtherTaxValue = selectedTax.TaxRate;
                    //    TaxAmount = ((selectedTax.TaxRate) / (100)) * ps.TotalSaleAmount;
                    //}

                    SaleOrderItem ObjSaleOrderItem = new SaleOrderItem();

                    ObjSaleOrderItem.ProductId = item.product;
                    ObjSaleOrderItem.Quantity = item.qty; 
                    ObjSaleOrderItem.TaxAmount = TaxAmount;
                    ObjSaleOrderItem.DateAdded = DateTime.Now;
                    ObjSaleOrderItem.SalePrice = selectedProduct.SalePrice;
                    ObjSaleOrderItem.TotalAmount = ps.TotalSaleAmount;
                    ObjSaleOrderItem.TotalAmountWithTax = ps.TotalSaleAmountWithTax;
                    ObjSaleOrderItem.TaxId = selectedTax.Id;
                    ObjSaleOrderItem.SaleOrderId= ObjSaleOrder.Id;
                    db.SaleOrderItems.Add(ObjSaleOrderItem);

                    db.SaveChanges(userId);
                }
                                       
                    result = "Done";
                    return Json(result, JsonRequestBehavior.AllowGet);
               
                
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }

            return Content(sb.ToString());
        }

        // POST: /SaleOrder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateInput(false)]
        //public ActionResult Create(SaleOrder ObjSaleOrder)
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {


        //            db.SaleOrders.Add(ObjSaleOrder);
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

        // GET: SaleOrder/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SaleOrder ObjSaleOrder = db.SaleOrders.Find(id);
            if (ObjSaleOrder == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.SaleOrders, "Id", "Name", ObjSaleOrder.Id);

            return View(ObjSaleOrder);
        }

        // POST: SaleOrder/Edit/5        
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(SaleOrder ObjSaleOrder)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(ObjSaleOrder).State = EntityState.Modified;
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
            SaleOrder ObjSaleOrder = db.SaleOrders.Find(id);
            if (ObjSaleOrder == null)
            {
                return HttpNotFound();
            }
            return View(ObjSaleOrder);
        }

        // POST: /SaleOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                SaleOrder ObjSaleOrder = db.SaleOrders.Find(id);
                db.SaleOrders.Remove(ObjSaleOrder);
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

        // GET: /SaleOrder/MultiViewIndex/5
        public ActionResult MultiViewIndex(int? id)
        {
            SaleOrder ObjSaleOrder = db.SaleOrders.Find(id);
            ViewBag.IsWorking = 0;
            if (id > 0)
            {
                ViewBag.IsWorking = id;
                ViewBag.ParentId = new SelectList(db.SaleOrders, "Id", "MenuText", ObjSaleOrder.Id);

            }

            return View(ObjSaleOrder);
        }

        // POST: /SaleOrder/MultiViewIndex/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MultiViewIndex(SaleOrder ObjSaleOrder)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {
                if (ModelState.IsValid)
                {


                    db.Entry(ObjSaleOrder).State = EntityState.Modified;
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
        public class OrderSaleItem
        {
            public int? productId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal price { get; set; }
            public decimal totalAmount { get; set; }
            public decimal qty { get; set; }
            public int? taxId { get; set; }
            public decimal taxAmount { get; set; }
            public decimal totalAmountWithTax { get; set; }
            public int? saleOrderId { get; set; }
            public decimal remainingquantity { get; set; }
        }
    }
}

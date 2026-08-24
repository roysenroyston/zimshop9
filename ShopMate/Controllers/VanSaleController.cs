using ShopMate.ModelDto;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;
using static ShopMate.Controllers.InvoiceController;

namespace ShopMate.Controllers
{
    public class VanSaleController : Controller
    {
        string userId = Env.GetUserInfo("name");
        private SIContext db = new SIContext();

        public ActionResult GetGrid()
        {
            var tak = db.VanSales.Where(i => i.IsCanceled == false).ToArray();

            var result = from c in tak
                         select new string[] { c.Id.ToString(), Convert.ToString(c.Id),
            Convert.ToString(c.User_UserId.UserName),
            Convert.ToString(c.DateAdded),
            Convert.ToString(c.IsCanceled),
            Convert.ToString(c.Route),
           // Convert.ToString(c.approved),
             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        // GET: Van
        public ActionResult Index()
        {
            return View(db.VanSales.ToList());
        }



        [HttpGet]
        // GET: VanSale/CancelVanSale/5
        public ActionResult CancelVanSale(int? id)
        {
            VanSale vansale = db.VanSales.Find(id);
            var vansaleitems = db.VanSaleItems.Where(q => q.VanSaleId == id).ToArray();

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            VanSaleDto dto = new VanSaleDto();
            if (vansale == null)
            {
                return HttpNotFound();

            }
            //string yes = vansale.IsCanceled.ToString();
            //if (vansale.IsCanceled == true)
            //{ return HttpNotFound(); }
            //else
            //{

                
                dto.Warehouse = db.Warehouses.FirstOrDefault(i => i.Id == vansale.WarehouseId).Name;
                dto.StockValue = vansale.StockValue;
                dto.Id = vansale.Id;
                dto.DateAdded = vansale.DateAdded;
                dto.Van = vansale.Van_VanId.RegNumber;
                dto.Driver = vansale.Driver;

                List<VanSaleItemDto> itemsList = new List<VanSaleItemDto>();

                foreach (var items in vansaleitems)
                {
                    var selectedVanSaleItem = db.VanSaleItems.FirstOrDefault(i => i.Id == items.Id);

                    var selectedProduct = db.Products.FirstOrDefault(i => i.Id == db.VanSaleItems.FirstOrDefault(v => v.Id == items.Id).ProductId);



                    VanSaleItemDto itemDto = new VanSaleItemDto();
                    //itemDto.Id = items.Id;
                    itemDto.OpeningStock = items.OpeningStock;
                    itemDto.Product = items.Product_ProductId.Name;
                    itemDto.SalePrice = items.SalePrice;
                    itemDto.Sales = items.Sales;
                    itemDto.StockAmount = items.StockAmount;
                    itemDto.StockValue = items.StockValue;
                    itemDto.UnitPrice = items.UnitPrice;
                    itemDto.VanSaleId = items.VanSaleId;
                    itemsList.Add(itemDto);
                    //selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity + (selectedVanSaleItem.OpeningStock);


                    //db.Entry(selectedProduct).State = EntityState.Modified;
                    db.SaveChanges(userId);
                }

                dto.items = itemsList;
                //vansale.IsCanceled = true;
                //db.Entry(vansale).State = EntityState.Modified;
                db.SaveChanges(userId);
            //}
            return View(dto);
        }

  
            // GET: VanSale/ReturnVanSale/5
            public ActionResult ReturnVanSale(int? id)
        {
            VanSale vansale = db.VanSales.Find(id);
            var vansaleitems = db.VanSaleItems.Where(q => q.VanSaleId == id).ToArray();
           // ViewBag.Returned = vansale.IsReturned;
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            if (vansale == null)
            {
                return HttpNotFound();
            }

            VanSaleDto dto = new VanSaleDto();
            dto.Warehouse = db.Warehouses.FirstOrDefault(i => i.Id == vansale.WarehouseId).Name;
            dto.StockValue = vansale.StockValue;
            dto.Id = vansale.Id;
            dto.DateAdded = vansale.DateAdded;
            dto.Van = vansale.Van_VanId.RegNumber;
            dto.Driver = vansale.Driver;

            List<VanSaleItemDto> itemsList = new List<VanSaleItemDto>();

            foreach (var items in vansaleitems)
            {
                VanSaleItemDto itemDto = new VanSaleItemDto();
                itemDto.Id = items.Id;
                itemDto.OpeningStock = items.OpeningStock;
                itemDto.ClosingStock = (int)items.ClosingStock;
                itemDto.Product = items.Product_ProductId.Name;
                itemDto.SalePrice = items.SalePrice;
                itemDto.Sales = items.Sales;
                itemDto.StockAmount = items.StockAmount;
                itemDto.StockValue = items.StockValue;
                itemDto.UnitPrice = items.UnitPrice;
                itemDto.VanSaleId = items.VanSaleId;
                itemDto.GP = items.GP;
                itemDto.OverallGP = (decimal)items.OverallGP;
                itemDto.GoodsSold = items.GoodsSold;
                itemsList.Add(itemDto);
            }

            dto.items = itemsList;

            return View(dto);
        }
        [HttpPost]
        [ValidateInput(false)]
        //public JsonResult ReturnVanSale1(int? Id, List<Cart> productss)
        public JsonResult ReturnVanSale1(int? Id,  List<Cart> productss)
        {
            VanSale ObjVanSale = db.VanSales.Find(Id);
            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
            //string CustomerName = db.Users.FirstOrDefault(i => i.Id == AddedBy).UserName;
            int warehouse = ObjVanSale.WarehouseId;
       
            List<VanSaleItem> LstVanSaleItem = new List<VanSaleItem>();
            string retVal = "";

            try {
                Invoice inv = new Invoice();
                inv.AddedBy = AddedBy;
                inv.DateAdded = DateTime.Now;
                inv.DateModied = DateTime.Now;
                inv.IsBilled = false;
                inv.IsPurchaseOrSale = "Sale";
                inv.ModifiedBy = AddedBy;
                inv.UserId = AddedBy;
                inv.WarehouseId = warehouse;
                db.Invoices.Add(inv);

                Sale ObjSale = new Models.Sale();
                foreach (var item in productss)
                {

                    var selectedVanSaleItem = db.VanSaleItems.FirstOrDefault(i => i.Id == item.Id);
                   var ObjWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == db.VanSaleItems.FirstOrDefault(v => v.Id == item.Id).ProductId && i.WarehouseId ==  warehouse);
                    var selectedProduct = db.Products.FirstOrDefault(i => i.Id == db.VanSaleItems.FirstOrDefault(v => v.Id == item.Id).ProductId);

                    var Item = db.VanSaleItems.FirstOrDefault(i => i.Id == item.Id);
                    if (Item.IsReturned == true)
                    {
                        retVal = "Duplicate Return is not allowed!!";
                        return Json(retVal, JsonRequestBehavior.AllowGet);
                    }
                    //item.ClosingStock = 5;
                    Item.ClosingStock = item.ClosingStock;
                  
                    Item.GoodsSold = Item.OpeningStock - item.ClosingStock;
                    Item.Sales = Item.GoodsSold * Item.SalePrice;
                    Item.GP = Item.SalePrice - selectedVanSaleItem.Product_ProductId.PurchasePrice;
                    Item.OverallGP = Item.GP * Item.GoodsSold;
                    Item.StockValue = Item.ClosingStock * Item.SalePrice;
                    Item.IsReturned = true;
                    db.Entry(Item).State = EntityState.Modified;
                    db.SaveChanges(userId);
                    //selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity + (item.ClosingStock);

                    ObjWarehouseStock.RemainingQuantity = ObjWarehouseStock.RemainingQuantity - (item.ClosingStock);
                    db.Entry(ObjVanSale).State = EntityState.Modified;
                    db.Entry(ObjWarehouseStock).State = EntityState.Modified;
                    db.Entry(selectedProduct).State = EntityState.Modified;
                    db.SaveChanges(userId);
                }
                //VanSale returns = db.VanSales.FirstOrDefault(i => i.Id == Id);
                //returns.IsReturned = true;
                //db.Entry(returns).State = EntityState.Modified;
                //    db.SaveChanges(userId);
                //retVal = "tapedza";
                retVal = "Success";
                return Json(retVal, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                retVal = "error:" + ex.Message;
            }

            return Json(retVal, JsonRequestBehavior.AllowGet);
        }


            // GET: VanSale/Details/5
            public ActionResult Details(int? id)
        {
            VanSale vansale = db.VanSales.Find(id);
            var vansaleitems = db.VanSaleItems.Where(q => q.VanSaleId == id).ToArray();

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            if (vansale == null)
            {
                return HttpNotFound();
            }

            VanSaleDto dto = new VanSaleDto();
            dto.Warehouse = db.Warehouses.FirstOrDefault(i => i.Id == vansale.WarehouseId).Name;
            dto.StockValue = vansale.StockValue;
            dto.StockValueRtgs = vansale.StockValueRtgs;
            dto.Id = vansale.Id;
            dto.DateAdded = vansale.DateAdded;
            dto.Van = vansale.Van_VanId.RegNumber;
            dto.Driver = vansale.Driver;
            dto.Route = vansale.Route;

            List<VanSaleItemDto> itemsList = new List<VanSaleItemDto>();

            foreach (var items in vansaleitems)
            {
                VanSaleItemDto itemDto = new VanSaleItemDto();
                //itemDto.Id = items.Id;
                itemDto.OpeningStock = items.OpeningStock;
                itemDto.ClosingStock = (int)items.ClosingStock;
                itemDto.Product = items.Product_ProductId.Name;
                itemDto.SalePrice = items.SalePrice;
                itemDto.SalePriceRtgs = items.SalePriceRtgs;
                itemDto.Sales = items.Sales;
                itemDto.StockAmount = items.StockAmount;
                itemDto.StockValue = items.StockValue;
                itemDto.StockValueRtgs = items.StockValueRtgs;
                itemDto.UnitPrice = items.UnitPrice;
                itemDto.VanSaleId = items.VanSaleId;
                itemsList.Add(itemDto);
            }

            dto.items = itemsList;

            return View(dto);
        }
        // GET: VanSale/Create
        public ActionResult Create()
        {
            ViewBag.VanId = new SelectList(db.Vans, "Id", "RegNumber");
            ViewBag.ProductId = new SelectList(db.Products.Where(p => p.RemainingQuantity > 0), "Id", "Name");
            ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            ViewBag.Driver = new SelectList(db.Users.Where(i => i.RoleId == 2 && i.CanLogin == true), "FullName", "FullName");



            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(int VanId, string Driver, decimal StockValue, decimal StockValueRtgs, string Route, VanSaleItem[] productss)
        {
            int WarehouseId = (int)db.Users.Where(t => t.FullName == Driver).FirstOrDefault().WarehouseId;
            int AddedBy = Convert.ToInt32(Env.GetUserInfo("userid"));
          //  int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            string result = "Error! Van Sale Is Not Complete!";
            try
            {
                
             

                try
                {

                    VanSale inv = new VanSale();
                    inv.UserId = AddedBy;
                    inv.StockValue = StockValue;
                    inv.StockValueRtgs = StockValueRtgs;
                    inv.DateAdded = DateTime.Now;
                    inv.VanId = VanId;
                    inv.Driver = Driver;
                    inv.Route = Route;
                    inv.WarehouseId = (int)WarehouseId;
                    db.VanSales.Add(inv);
                    inv.IsCanceled = false;

                    db.SaveChanges(userId);
                    string tableData = "";
                    foreach (var item in productss)
                    {

                        var selectedProduct = db.Products.FirstOrDefault(i => i.Id == item.ProductId);
                        var ObjWarehouseStock = db.WarehouseStocks.FirstOrDefault(i => i.ProductId == item.ProductId && i.WarehouseId == WarehouseId);

                        if (selectedProduct.RemainingQuantity < item.OpeningStock)
                        {
                            result = "not enough stock to dispatch for " + selectedProduct.Name;
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            selectedProduct.RemainingQuantity = selectedProduct.RemainingQuantity - item.OpeningStock;
                            ObjWarehouseStock.RemainingQuantity = ObjWarehouseStock.RemainingQuantity + item.OpeningStock;
                            db.Entry(selectedProduct).State = EntityState.Modified;
                            db.SaveChanges(userId);

                        }
                        VanSaleItem Iitem = new VanSaleItem();
                        Iitem.ProductId = item.ProductId;
                        Iitem.OpeningStock = item.OpeningStock;
                        Iitem.ClosingStock = 0;
                        Iitem.SalePriceRtgs = item.SalePriceRtgs;
                        Iitem.SalePrice = item.SalePrice;
                        Iitem.Description = item.Description;
                        //Iitem.ClosingStock = item.ClosingStock;
                        Iitem.UnitPrice = selectedProduct.PurchasePrice;
                        Iitem.StockAmount = 0;
                        Iitem.StockValue = StockValue;
                        item.StockValueRtgs = StockValueRtgs;
                        Iitem.OverallGP = 0;
                        Iitem.GP = item.SalePrice - selectedProduct.PurchasePrice;
                        Iitem.Sales = 0;
                        Iitem.GoodsSold = 0;
                        Iitem.DateAdded = DateTime.Now;
                        Iitem.VanSaleId = inv.Id;
                        db.VanSaleItems.Add(Iitem);
                        db.SaveChanges(userId);

                        tableData +=
                           "<tr>" +
                           "<td>" + item.Description + "</td>" +
                           "<td>" + item.OpeningStock + "</td>" +
                           "<td>" + item.SalePrice + "</td>" +
                           "<td>" + (selectedProduct.RtgsPrice ) + "</td>" +
                           "<td>" + (item.SalePrice * item.OpeningStock) + "</td>" +
                           "<td>" + (selectedProduct.RtgsPrice * item.OpeningStock) + "</td>" +
                           "</tr>";


                    }
                    string[] emails = { "winston@zimhope.co.zw", "trynosmuch@gmail.com", "ngonidzashe@zimhope.co.zw", "winstonkaseke@live.com", "faithkaseke53@gmail.com" };
                    var body = System.IO.File.ReadAllText(System.Web.HttpContext.Current.Server.MapPath("/Views/Mail/vancreate.mail.htm"));
                    body = string.Format(body,
                        "New Van Sell : " + db.Vans.Find(VanId).RegNumber
                        , db.Users.Find(AddedBy).UserName,
                        DateTime.Now,
                        inv.Id,
                        db.Vans.Find(VanId).RegNumber,
                        db.Warehouses.Find(WarehouseId).Name,
                        Driver,
                        Route,
                        StockValue,
                        StockValueRtgs,
                        tableData
                        );
                    _ = Env.sendMail(emails, body, "New Van Sell");
                    result = "Success! Van Sale Completed";
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

        // GET: Van/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VanSale ObjVan = db.VanSales.Find(id);
            if (ObjVan == null)
            {
                return HttpNotFound();
            }


            return View(ObjVan);
        }

        // POST: Van/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.


        public ActionResult Edit(VanSale ObjVan)
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
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                VanSale ObjVanSale = db.VanSales.Find(id);
                ObjVanSale.IsCanceled = true;
                db.Entry(ObjVanSale).State = EntityState.Modified;
                db.SaveChanges(userId);

                sb.Append("Successfully Cancelled");
                GetGrid();
                //return Content(sb.ToString());

            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }

            //return Content(sb.ToString());
            return RedirectToAction("Index", "VanSale");


        }

        // POST: VanSale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            try
            {

                VanSale ObjVanSale = db.VanSales.Find(id);
                var vanSaleItems = db.VanSaleItems.Where(q => q.VanSaleId == id).ToArray();

                int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
                var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
                VanSaleDto dto = new VanSaleDto();
                if (ObjVanSale == null)
                {
                    return HttpNotFound();

                }
                else
                {
                    if (ObjVanSale.IsCanceled == true) {
                        sb.Append("Van Sale Already Cancelled!!!");
                        return Content(sb.ToString());
                    }
                    else
                    {
                        foreach (var item in vanSaleItems)
                        {
                            var selectedproduct = db.Products.Find(item.ProductId);
                            selectedproduct.RemainingQuantity = selectedproduct.RemainingQuantity + item.OpeningStock;
                            db.Entry(selectedproduct).State = EntityState.Modified;
                            db.SaveChanges(userId);
                        }
                        ObjVanSale.IsCanceled = true;
                        db.Entry(ObjVanSale).State = EntityState.Modified;
                        db.SaveChanges(userId);
                        //db.VanSales.Remove(ObjVanSale);
                        db.SaveChanges(userId);

                        sb.Append("Submitted");
                    }
                }
                //return Content(sb.ToString());
                //return RedirectToAction("Index", "VanSale");

                                                          
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                sb.Append("Error :" + ex.Message);
            }

            //return Content(sb.ToString());
            return RedirectToAction("Index", "VanSale");
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

        public class Cart
        {
            public int Id { get; set; }
            public int ClosingStock { get; set; }
        }


        public ActionResult print(int id)
        {
            VanSale vansale = db.VanSales.Find(id);
            var vansaleitems = db.VanSaleItems.Where(q => q.VanSaleId == id).ToArray();

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            if (vansale == null)
            {
                return HttpNotFound();
            }

            VanSaleDto dto = new VanSaleDto();
            dto.Warehouse = db.Warehouses.FirstOrDefault( i=> i.Id==  vansale.WarehouseId).Name;
            dto.StockValue = vansale.StockValue;
            dto.StockValueRtgs = vansale.StockValueRtgs;
            dto.Id = vansale.Id;
            dto.DateAdded = vansale.DateAdded;
            dto.Van = vansale.Van_VanId.RegNumber;
            dto.Driver = vansale.Driver;

            List<VanSaleItemDto> itemsList = new List<VanSaleItemDto>();

            foreach (var items in vansaleitems)
            {
                VanSaleItemDto itemDto = new VanSaleItemDto();
                //itemDto.Id = items.Id;
                itemDto.OpeningStock = items.OpeningStock;
                itemDto.Product = items.Product_ProductId.Name;
                itemDto.SalePrice = items.SalePrice;
                itemDto.Sales = items.Sales;
                itemDto.StockAmount = items.StockAmount;
                itemDto.StockValue = items.StockValue;
                itemDto.StockValueRtgs = items.StockValueRtgs;
                itemDto.UnitPrice = items.UnitPrice;
                itemDto.VanSaleId = items.VanSaleId;
                itemsList.Add(itemDto);
            }

            dto.items = itemsList;

            return View(dto);

        }

    }
}
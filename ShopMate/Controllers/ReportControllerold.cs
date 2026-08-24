using ShopMate.ModelDto;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class ReportController : Controller
    {
        private SIContext db = new SIContext();

        //
        [HttpGet]
        public ActionResult cashierreport()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var till = db.Users.FirstOrDefault(i => i.WarehouseId == warehouse );
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            List<CashupDto> listStock = new List<CashupDto>();

            var sAlert1 = db.InvoiceItemss.Where(i => i.WarehouseId == warehouse && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
               .Select(i => new { Tilloperator=i.AddedBy, totalsalesfortheday = i.TotalAmountWithTax}).ToArray();
            var ct = db.Products.OrderBy(i => i.ProductCategoryId);
            var payments = db.Paymenttracks.Where(i => i.WarehouseId == warehouse && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                 .Select(i => new { totalcash = i.cash, totalecocash=i.ecocash, Totalswipe = i.swipe, Dated=i.DateAdded, Tilloperator = i.AddedBy }).ToArray();
            var accountpayment = db.AccountPayments.Where(i => i.WarehouseId == warehouse && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                 .Select(i => new { accountpayments = i.Amount, cashs = i.cash, ecocashs = i.ecocash, swipes = i.swipe, Dated = i.DateAdded, Tilloperator = i.AddedBy }).ToArray();
            var accountsale = db.ProductStock.Where(i => i.WarehouseId == warehouse && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                 .Select(i => new { accountsales = i.TotalSaleAmountWithTax, Tilloperator = i.AddedBy }).ToArray();


            User dst = new User();
            var sAlert2 = sAlert1.Select(i => i.Tilloperator).Distinct();
            foreach (var item in sAlert2)
            {
                CashupDto li = new CashupDto();
                var selectedProduct = db.Users.FirstOrDefault(i => i.Id == item);
                li.TilloperatorName = selectedProduct.UserName;
                li.totalsalesfortheday = sAlert1.Where(i =>i.Tilloperator == item).Sum(i => i.totalsalesfortheday);
                li.totalcash = payments.Where(i => i.Tilloperator == item).Sum(i => i.totalcash)+ accountpayment.Where(i => i.Tilloperator == item).Sum(i => i.cashs);
                li.Totalswipe = payments.Where(i => i.Tilloperator == item).Sum(i => i.Totalswipe) + accountpayment.Where(i => i.Tilloperator == item).Sum(i => i.swipes); ;
                li.totalecocash = payments.Where(i => i.Tilloperator == item).Sum(i => i.totalecocash) + accountpayment.Where(i => i.Tilloperator == item).Sum(i => i.ecocashs); ;
                li.accountpayments = accountpayment.Where(i => i.Tilloperator == item).Sum(i => i.accountpayments);
                li.accountsales = accountsale.Where(i => i.Tilloperator == item).Sum(i => i.accountsales);

                listStock.Add(li);
            }
            ViewBag.company = invoiceFormat.CompanyName;
            ViewBag.UserId = new SelectList(db.Products, "Id", "Name");
            return View(listStock);
        }


        [HttpPost]
        public ActionResult cashierreport(string FromDate, string ToDate, string stime,string etime)
        {

            var Datefrom = Env.AddTimeInDate(DateTime.Parse(FromDate), stime);
             var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            

        BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var till = db.Users.FirstOrDefault(i => i.WarehouseId == warehouse);
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            List<CashupDto> listStock = new List<CashupDto>();

            var sAlert1 = db.InvoiceItemss.Where(i => i.WarehouseId == warehouse && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
               .Select(i => new { Tilloperator = i.AddedBy, totalsalesfortheday = i.TotalAmountWithTax }).ToArray();
            var ct = db.Products.OrderBy(i => i.ProductCategoryId);
            var payments = db.Paymenttracks.Where(i => i.WarehouseId == warehouse && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                 .Select(i => new { totalcash = i.cash, totalecocash = i.ecocash, Totalswipe = i.swipe, Dated = i.DateAdded, Tilloperator = i.AddedBy }).ToArray();
            var accountpayment = db.AccountPayments.Where(i => i.WarehouseId == warehouse && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                 .Select(i => new { accountpayments = i.Amount, cashs = i.cash, ecocashs = i.ecocash, swipes = i.swipe, Dated = i.DateAdded, Tilloperator = i.AddedBy }).ToArray();
            var accountsale = db.ProductStock.Where(i => i.WarehouseId == warehouse && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                 .Select(i => new { accountsales = i.TotalSaleAmountWithTax, Tilloperator = i.AddedBy }).ToArray();


            User dst = new User();
            var sAlert2 = sAlert1.Select(i => i.Tilloperator).Distinct();
            foreach (var item in sAlert2)
            {
                CashupDto li = new CashupDto();
                var selectedProduct = db.Users.FirstOrDefault(i => i.Id == item);
                li.TilloperatorName = selectedProduct.UserName;
                li.totalsalesfortheday = sAlert1.Where(i => i.Tilloperator == item).Sum(i => i.totalsalesfortheday)+ accountsale.Where(i => i.Tilloperator == item).Sum(i => i.accountsales); ;
                li.totalcash = payments.Where(i => i.Tilloperator == item).Sum(i => i.totalcash) + accountpayment.Where(i => i.Tilloperator == item).Sum(i => i.cashs);
                li.Totalswipe = payments.Where(i => i.Tilloperator == item).Sum(i => i.Totalswipe) + accountpayment.Where(i => i.Tilloperator == item).Sum(i => i.swipes); ;
                li.totalecocash = payments.Where(i => i.Tilloperator == item).Sum(i => i.totalecocash) + accountpayment.Where(i => i.Tilloperator == item).Sum(i => i.ecocashs); ;
                li.accountpayments = accountpayment.Where(i => i.Tilloperator == item).Sum(i => i.accountpayments);
                li.accountsales = accountsale.Where(i => i.Tilloperator == item).Sum(i => i.accountsales);

                listStock.Add(li);
            }
            ViewBag.company = invoiceFormat.CompanyName;
            ViewBag.UserId = new SelectList(db.Products, "Id", "Name");
            return View(listStock);
        }
        public ActionResult formalreport()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            SaleDto[] sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalSaleAmount, WithTaxAmount = i.TotalSaleAmountWithTax, Dated = i.DateAdded.Value }).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }
        // GET: /Report/
        [HttpGet]
        public ActionResult TodaySale()
        { 
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto,1);

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            SaleDto[] sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalSaleAmount, WithTaxAmount = i.TotalSaleAmountWithTax, Dated = i.DateAdded.Value }).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }
        [HttpGet]
        public ActionResult TodayRawMaterialPurchase()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.RawMaterial, "Id", "Name");

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            RawMaterialDto[] sale = db.RawMaterialStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 7 && i.WarehouseId == warehouse)
                .Select(i => new RawMaterialDto { Name = i.RawMaterials_RawMaterialsId.Name, Quantity = i.Quantity, PurchasePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, Dated = i.DateAdded.Value }).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }
        [HttpPost]
        public ActionResult TodayRawMaterialPurchase(string FromDate, string ToDate, string stime, string etime, int? ProductId = null)
        {

            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);


            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            var InventoryTypeId = db.InventoryTypes.FirstOrDefault(i => i.Name == "Purchase").Id;

            List<RawMaterialDto> sale = new List<RawMaterialDto>();

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.RawMaterial, "Id", "Name", ProductId);
                sale = db.RawMaterialStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == InventoryTypeId && i.RawMaterialsId == ProductId && i.WarehouseId == warehouse)
                .Select(i => new RawMaterialDto { Name = i.RawMaterials_RawMaterialsId.Name, Quantity = i.Quantity, PurchasePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, Dated = i.DateAdded.Value }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.RawMaterial, "Id", "Name");
                sale = db.RawMaterialStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == InventoryTypeId && i.WarehouseId == warehouse)
                .Select(i => new RawMaterialDto { Name = i.RawMaterials_RawMaterialsId.Name, Quantity = i.Quantity, PurchasePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, Dated = i.DateAdded.Value }).ToList();
            }
            return View(sale);


        }
        [HttpGet]
        public ActionResult Grv()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            GrvDto[] Purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.WarehouseId == warehouse)
                .Select(i => new GrvDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.UnitPrice, Amount = i.TotalAmount, Supplier = i.User_VendorUserId.UserName, Dated = i.DateAdded }).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(Purchase);
        }

        [HttpPost]
        public ActionResult Grv(string FromDate, string ToDate, string stime, string etime, int? ProductId = null)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));

            List<GrvDto> Purchase = new List<GrvDto>();

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                Purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                .Select(i => new GrvDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.UnitPrice, Amount = i.TotalAmount, Supplier = i.User_VendorUserId.UserName, Dated = i.DateAdded }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                Purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.WarehouseId == warehouse)
                .Select(i => new GrvDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.UnitPrice, Amount = i.TotalAmount, Supplier = i.User_VendorUserId.UserName, Dated = i.DateAdded }).ToList();
            }


            return View(Purchase);
        }

        [HttpGet]
        public ActionResult Shrinkage()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            ShrinkageDto[] sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && (i.InventoryTypeId == 5 || i.InventoryTypeId == 6 ||i.InventoryTypeId==7) && i.WarehouseId == warehouse)
                .Select(i => new ShrinkageDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, effect = i.InventoryType_InventoryTypeId.Name, Description = i.Description, WithTaxAmount = i.TotalSaleAmountWithTax, Dated = i.DateAdded.Value }).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }
        [HttpPost]
        public ActionResult Shrinkage(string FromDate, string ToDate, string stime, string etime, int? ProductId = null)
        {

            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);


            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            List<ShrinkageDto> sale = new List<ShrinkageDto>();

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && (i.InventoryTypeId == 5 || i.InventoryTypeId == 7 || i.InventoryTypeId == 6) && i.WarehouseId == warehouse)
                .Select(i => new ShrinkageDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, effect = i.InventoryType_InventoryTypeId.Name, Description = i.Description, WithTaxAmount = i.TotalSaleAmountWithTax, Dated = i.DateAdded.Value }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && (i.InventoryTypeId == 5 || i.InventoryTypeId == 7 || i.InventoryTypeId == 6) && i.WarehouseId == warehouse)
                .Select(i => new ShrinkageDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, effect = i.InventoryType_InventoryTypeId.Name, Description = i.Description, WithTaxAmount = i.TotalSaleAmountWithTax, Dated = i.DateAdded.Value }).ToList();
            }
            return View(sale);


        }

        [HttpGet]
        public ActionResult TodayRawMaterialUse()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            var InventoryTypeId = db.InventoryTypes.FirstOrDefault(i => i.Name == "Raw Materials Out").Id;

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.RawMaterial, "Id", "Name");

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            RawMaterialDto[] sale = db.RawMaterialStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == InventoryTypeId && i.WarehouseId == warehouse)
                .Select(i => new RawMaterialDto { Name = i.RawMaterials_RawMaterialsId.Name, Quantity = i.Quantity, PurchasePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, Dated = i.DateAdded.Value }).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }
        [HttpGet]
        public ActionResult manufacturing()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            var InventoryTypeId = db.InventoryTypes.FirstOrDefault(i => i.Name == "Raw Materials Out").Id;

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            ManufacturingDto[] sale = db.FinishedItems.Where(i => (i.dateadded >= Datefrom && i.dateadded <= Dateto)  && i.WarehouseId == warehouse)
                .Select(i => new ManufacturingDto { Name = i.Product_ProductId.Name, Quantity = i.Quantity, unitprice = i.unitprice, Total = i.Total, Dated = i.dateadded , warehouseid = i.WarehouseId }).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }
        [HttpPost]
        public ActionResult manufacturing(string FromDate, string ToDate, string stime, string etime, int? ProductId = null)
        {

            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);


            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            List<ManufacturingDto> sale = new List<ManufacturingDto>();

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                sale = db.FinishedItems.Where(i => (i.dateadded >= Datefrom && i.dateadded <= Dateto) && i.ProductId == ProductId && i.WarehouseId == warehouse)
                .Select(i => new ManufacturingDto { Name = i.Product_ProductId.Name, Quantity = i.Quantity, unitprice = i.unitprice, Total = i.Total, Dated = i.dateadded, warehouseid = i.WarehouseId }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                sale = db.FinishedItems.Where(i => (i.dateadded >= Datefrom && i.dateadded <= Dateto)  && i.WarehouseId == warehouse)
                .Select(i => new ManufacturingDto { Name = i.Product_ProductId.Name, Quantity = i.Quantity, unitprice = i.unitprice, Total = i.Total, Dated = i.dateadded, warehouseid = i.WarehouseId }).ToList();
            }
            return View(sale);


        }

        [HttpPost]
        public ActionResult TodayRawMaterialUse(string FromDate, string ToDate, string stime, string etime, int? ProductId = null)
        {

            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);


            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            var InventoryTypeId = db.InventoryTypes.FirstOrDefault(i => i.Name == "Raw Materials Out").Id;

            List<RawMaterialDto> sale = new List<RawMaterialDto>();

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.RawMaterial, "Id", "Name", ProductId);
                sale = db.RawMaterialStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == InventoryTypeId && i.RawMaterialsId == ProductId && i.WarehouseId == warehouse)
                .Select(i => new RawMaterialDto { Name = i.RawMaterials_RawMaterialsId.Name, Quantity = i.Quantity, PurchasePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, Dated = i.DateAdded.Value }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.RawMaterial, "Id", "Name");
                sale = db.RawMaterialStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == InventoryTypeId && i.WarehouseId == warehouse)
                .Select(i => new RawMaterialDto { Name = i.RawMaterials_RawMaterialsId.Name, Quantity = i.Quantity, PurchasePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, Dated = i.DateAdded.Value }).ToList();
            }
            return View(sale);


        }

        [HttpPost]
        public ActionResult TodaySale(string FromDate, string ToDate, string stime, string etime, int? ProductId=null, int? IsFormalId = null)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 1);

            //var text = db.InvoiceTypes.FirstOrDefault(i => i.Id == (IsFormalId)).Name;
            //var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            //var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);
            //////var myDate = FromDate + " " + stime;
            //////var yourDate = ToDate + " " + etime;
            //////CultureInfo provider = CultureInfo.InvariantCulture;
            //////DateTime Datefrom; // 1/1/0001 12:00:00 AM  
            //////DateTime Dateto; // 1/1/0001 12:00:00 AM 
            //////bool isSuccess4 = DateTime.TryParseExact(myDate, "MM-dd-yyyy HH:mm", provider, DateTimeStyles.None, out Datefrom);
            //////bool isSuccess3 = DateTime.TryParseExact(yourDate, "MM-dd-yyyy HH:mm", provider, DateTimeStyles.None, out Dateto);


            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            List<SaleDto> sale = new List<SaleDto>();
            if(IsFormalId == null || db.InvoiceTypes.FirstOrDefault(i => i.Id == (IsFormalId)).Name == "ALL")  {
                       
            
            if (ProductId!=null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                    ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                    sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded.Value }).ToList(); 
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                    ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                    sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded.Value  }).ToList();
            }
            }
            else if(db.InvoiceTypes.FirstOrDefault(i => i.Id == (IsFormalId)).Name == "Formal")
            {


                if (ProductId != null)
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                    ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                    sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse && i.isFormalSale == true)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded.Value }).ToList();
                }
                else
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                    ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                    sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse && i.isFormalSale == true)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded.Value }).ToList();
                }
            }
            else if (db.InvoiceTypes.FirstOrDefault(i => i.Id == (IsFormalId)).Name == "Informal")
            {


                if (ProductId != null)
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                    ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                    sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse && i.isFormalSale == false)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded.Value }).ToList();
                }
                else
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                    ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                    sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse && i.isFormalSale == false)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded.Value }).ToList();

                }
            }

            return View(sale);
            
           
        }
        [HttpPost]
        public ActionResult formalreport(string FromDate, string ToDate, string stime, string etime, string IsFormalId, int? ProductId = null)
        {
            //ViewBag.IsFormalId = new SelectList(db.Products, "Id", "Name", ProductId);
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);
        
          

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            List<SaleDto> sale = new List<SaleDto>();
            if(IsFormalId == "" || IsFormalId == "ALL") {

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded.Value, }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalSaleAmountWithTax, Dated = i.DateAdded.Value }).ToList();
            }
            }
            else if (IsFormalId == "IsFormal")
            {
                if (ProductId != null)
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                    sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded.Value, }).ToList();
                }
                else
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                    sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalSaleAmountWithTax, Dated = i.DateAdded.Value }).ToList();
                }
            }
            else
            {
                if (ProductId != null)
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                    sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded.Value, }).ToList();
                }
                else
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                    sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalSaleAmountWithTax, Dated = i.DateAdded.Value }).ToList();
                }
            }
                return View(sale);


        }


        [HttpGet]
        public ActionResult TodayPurchase()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            SaleDto[] Purchase = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalPurchaseAmount+i.TaxAmount, Dated = i.DateAdded.Value }).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(Purchase);
        }

        [HttpPost]
        public ActionResult TodayPurchase(string FromDate, string ToDate, string stime, string etime, int? ProductId = null, int? IsFormalId = null)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            
            List<SaleDto> Purchase = new List<SaleDto>();

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                Purchase = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalPurchaseAmount + i.TaxAmount, Dated = i.DateAdded.Value }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                Purchase = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalPurchaseAmount + i.TaxAmount, Dated = i.DateAdded.Value }).ToList();
            }


            return View(Purchase);
        }
       
        [HttpGet]
        public ActionResult StockAlert()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            List<StockAlertDto> listStock = new List<StockAlertDto>();
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            var sAlert1 = db.ProductStock.Where(i => (i.InventoryTypeId == 1 || i.InventoryTypeId == 2 ||i.InventoryTypeId==5 || i.InventoryTypeId==6 ||i.InventoryTypeId==7) && i.WarehouseId == warehouse)
               .Select(i => new { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity,InvTypeId=i.InventoryTypeId,StockAlert=i.Product_ProductId.StockAlert}).ToArray();

            var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
            foreach (var item in sAlert2)
            {
                StockAlertDto li = new StockAlertDto();
                var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);
                li.ProductName = item;
                li.Quantity = db.Products.FirstOrDefault(i => i.Name == item).RemainingQuantity;
                //li.Quantity = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Quantity) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Quantity)- sAlert1.Where(i => i.InvTypeId == 7 && i.ProductName == item).Sum(i => i.Quantity) ;
                li.StockAlert = selectedProduct.StockAlert;
               // li.StockAlert = sAlert1.FirstOrDefault(i => i.InvTypeId == 1 && i.ProductName == item).StockAlert;
                listStock.Add(li);
            }
            ViewBag.company = invoiceFormat.CompanyName;
            return View(listStock);
        }
        [HttpGet]
        public ActionResult ExpiryAlert()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            List<ExpiryAlertDto> listStock = new List<ExpiryAlertDto>();// Do we have a product here?yes now m thinkin i did get expiry date i can get the rest of the infor that way ndoisa futi batch number . Ok let me try that
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            var sAlert1 = db.ProductStock.Where(i => (i.InventoryTypeId == 1 || i.InventoryTypeId == 2 || i.InventoryTypeId == 5 || i.InventoryTypeId == 6 || i.InventoryTypeId == 7) && i.WarehouseId == warehouse && i.ProductBatchId > 0 && i.ProductBatch_ProductBatchId.IsCleared == false)
               .Select(i => new { ProductName = i.Product_ProductId.Name, InvTypeId = i.InventoryTypeId, ExpiryAlert = i.Product_ProductId.ExpiryAlert, BatchNumberId = i.ProductBatch_ProductBatchId.BatchNumber, ExpiryDate = i.ProductBatch_ProductBatchId.ExpiryDate }).ToArray();

            var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
            foreach (var item in sAlert2)
            {
                ExpiryAlertDto li = new ExpiryAlertDto();
                var today = DateTime.Now;
                var ExpiryDate = sAlert1.FirstOrDefault(i => i.ProductName == item).ExpiryDate;
                var PeriodToExpiry = (today - sAlert1.FirstOrDefault(i => i.ProductName == item).ExpiryDate).Days;
                var itemBatchNumberIds = sAlert1.FirstOrDefault(i => i.ProductName == item).BatchNumberId;
                var ExpiryAlert = sAlert1.FirstOrDefault(i => i.ProductName == item).ExpiryAlert;
                if (PeriodToExpiry <= ExpiryAlert) { 
                var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);
                li.ProductName = item;
                li.PeriodToExpiry = PeriodToExpiry;
                li.BatchNumber = sAlert1.FirstOrDefault(i => i.ProductName == item).BatchNumberId;
                //li.Quantity = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Quantity) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Quantity)- sAlert1.Where(i => i.InvTypeId == 7 && i.ProductName == item).Sum(i => i.Quantity) ;
                //li.StockAlert = selectedProduct.StockAlert;
                // li.StockAlert = sAlert1.FirstOrDefault(i => i.InvTypeId == 1 && i.ProductName == item).StockAlert;
                listStock.Add(li);
                }
            }
            ViewBag.company = invoiceFormat.CompanyName;
            return View(listStock);
        }


        [HttpGet]
        public ActionResult StockAmount()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var products = db.Products.FirstOrDefault(i => i.WarehouseId == warehouse);
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            List<StockAmountDto> listStock = new List<StockAmountDto>();

            var sAlert1 = db.ProductStock.Where(i => (i.InventoryTypeId == 1 || i.InventoryTypeId == 2 || i.InventoryTypeId == 5 || i.InventoryTypeId == 3 || i.InventoryTypeId == 4 || i.InventoryTypeId == 6 || i.InventoryTypeId == 9) && i.WarehouseId == warehouse)
               .Select(i => new { ProductName = i.Product_ProductId.Name, Quantity = db.Products.FirstOrDefault(a => a.Id == i.ProductId).RemainingQuantity, InvTypeId = i.InventoryTypeId, cost = i.TotalPurchaseAmount, Amount = i.TotalSaleAmountWithTax }).ToArray();
            var ct = db.Products.OrderBy(i => i.ProductCategoryId);

            Product dst = new Product();
            var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
            foreach (var item in sAlert2)
            {
                StockAmountDto li = new StockAmountDto();
                var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);
                li.ProductName = item;
                li.Quantity = db.Products.FirstOrDefault(i => i.Name == item).RemainingQuantity;
                //li.Quantity = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Quantity)+ sAlert1.Where(i => i.InvTypeId == 4 && i.ProductName == item).Sum(i => i.Quantity) + sAlert1.Where(i => i.InvTypeId == 9 && i.ProductName == item).Sum(i => i.Quantity) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 3 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Quantity);
                // li.cost = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.cost) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.cost) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.cost) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.cost);
                // li.Amount = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Amount) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Amount) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Amount) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Amount);
                li.cost = Math.Round(li.Quantity * selectedProduct.PurchasePrice, 2);
                li.Amount = Math.Round(li.Quantity * selectedProduct.SalePrice, 2);
                li.purchaseprice = selectedProduct.PurchasePrice;
                li.saleprice = selectedProduct.SalePrice;
                listStock.Add(li);
            }
            ViewBag.company = invoiceFormat.CompanyName;
            return View(listStock);

        }



        [HttpPost]
        public ActionResult StockAmount(string FromDate, string ToDate, string stime, string etime, int? ProductId = null)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            List<StockAmountDto> listStock = new List<StockAmountDto>();

            var sAlert1 = db.ProductStock.Where(i => (i.InventoryTypeId == 1 || i.InventoryTypeId == 3 || i.InventoryTypeId == 4 || i.InventoryTypeId == 2 || i.InventoryTypeId == 5 || i.InventoryTypeId == 6 || i.InventoryTypeId == 9) && i.WarehouseId == warehouse)
               .Select(i => new { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, InvTypeId = i.InventoryTypeId, cost = i.TotalPurchaseAmount, Amount = i.TotalSaleAmountWithTax }).ToArray();
            var ct = db.Products.OrderBy(i => i.ProductCategoryId);

            Product dst = new Product();
            var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
            foreach (var item in sAlert2)
            {
                StockAmountDto li = new StockAmountDto();
                var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);
                li.ProductName = item;
                li.Quantity = db.Products.FirstOrDefault(i => i.Name == item).RemainingQuantity;
                //li.Quantity = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Quantity) + sAlert1.Where(i => i.InvTypeId == 4 && i.ProductName == item).Sum(i => i.Quantity) + sAlert1.Where(i => i.InvTypeId == 9 && i.ProductName == item).Sum(i => i.Quantity) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 3 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Quantity)- sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Quantity);
                //// li.cost = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.cost) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.cost) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.cost) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.cost);
                // li.Amount = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Amount) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Amount) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Amount) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Amount);
                li.cost = Math.Round(li.Quantity * selectedProduct.PurchasePrice, 2);
                li.Amount = Math.Round(li.Quantity * selectedProduct.SalePrice, 2);
                li.purchaseprice = selectedProduct.PurchasePrice;
                li.saleprice = selectedProduct.SalePrice;
                listStock.Add(li);
            }
            ViewBag.company = invoiceFormat.CompanyName;
            return View(listStock);
        }

        #region Accounts



        [HttpGet]
        public ActionResult Accounts()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            var catTre = db.LedgerAccounts.Where(i=>i.ParentId==null).ToArray();
           
            return View(catTre);
        }

        [HttpPost]
        public ActionResult Accounts(string FromDate, string ToDate, string stime, string etime, string productName)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);

            var catTre = db.LedgerAccounts.Where(i => i.ParentId == null).ToArray();
            return View(catTre);
        }

        public ContentResult GetPageControls(int id, string dfrom, string dto,string stime,string etime,int hit)
        {
            StringBuilder sbPrint = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            var catTre = db.LedgerAccounts.ToArray();
            var catTre1 = catTre.Where(i => i.ParentId == id || i.Id == id).ToArray();

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));

            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(dfrom), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(dto), etime);

            List<Transaction> tran = new List<Transaction>();

            if (hit == 0)
            {
                tran = db.Transactions.Where(i => (i.WarehouseId == warehouse) && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToList();
            }
            else
            {
                tran= db.Transactions.Where(i =>i.WarehouseId == warehouse).ToList();
            }
             
            sb.AppendLine("[");
            var parentGet = catTre1.Where(i => i.ParentId == null).ToArray();
            foreach (var item in parentGet)
            {
                //amount count sum 
                string val = amountCount(catTre, item, item.Id, tran.ToArray()).ToString();
                //

                sb.Append("{ \"id\":" + item.Id + ", \"text\":\"" + item.Name + "(" + val + ")" + "\"");
                var CheckInner = catTre.FirstOrDefault(j => j.ParentId == item.Id);
                if (CheckInner != null)
                {
                    sb.Append(child(catTre, item, item.Id, tran.ToArray()));
                }

                sb.Append("},");
            }

            var sbRem = sb.ToString().TrimEnd(',');
            sbPrint.Append(sbRem + "]");

            return new ContentResult { Content = sbPrint.ToString().Replace(",]", "]"), ContentType = "application/json" };
        }
         

        private static StringBuilder child(LedgerAccount[] catTre, LedgerAccount item, Nullable<int> ParentId, Transaction[] tran)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(",\"children\":[");
            foreach (var inner in catTre.Where(j => j.ParentId == ParentId))
            {
                var LAccount = catTre.Where(i => i.Id == inner.Id);
                decimal tranCredit = 0;
                decimal tranDebit = 0;
                try
                {
                    foreach (var lItem in LAccount)
                    {
                        tranCredit += tran.Where(i => i.CreditLedgerAccountId == lItem.Id).Sum(i => i.CreditAmount.Value);
                        //if (inner.Name == "Bank Accounts")
                        //{
                            tranDebit += tran.Where(i => i.DebitLedgerAccountId == lItem.Id).Sum(i => i.DebitAmount.Value);
                        //}

                    }
                }
                catch (Exception ex)
                {
                    Helper.WriteError(ex, ex.Message);
                }
                //if (inner.Name == "Bank Accounts")
                //{
                //    sb.Append("{ \"id\":" + inner.Id + ", \"text\":\"" + inner.Name + " (" + (tranCredit - tranDebit) + ")" + "\"");
                //}
                //else
                //{
                sb.Append("{ \"id\":" + inner.Id + ", \"text\":\"" + inner.Name + " (" + (tranCredit - tranDebit) + ")" + "\"");
                //}

                var CheckInner = catTre.FirstOrDefault(j => j.ParentId == inner.Id);
                if (CheckInner != null)
                {
                    sb.Append(child(catTre, CheckInner, inner.Id, tran));
                }
                sb.Append("},");
            }
            var sbChilePls = sb.ToString().TrimEnd(',');

            sb.Append("]");
            return sb;
        }

        public decimal amt = 0;

        private decimal amountCount(LedgerAccount[] catTre, LedgerAccount item, Nullable<int> ParentId, Transaction[] tran)
        {
            foreach (var inner in catTre.Where(j => j.ParentId == ParentId))
            {
                var LAccount = catTre.Where(i => i.Id == inner.Id);
                decimal tranCredit = 0;
                decimal tranDebit = 0;
                try
                {
                    foreach (var lItem in LAccount)
                    {
                        tranCredit += tran.Where(i => i.CreditLedgerAccountId == lItem.Id).Sum(i => i.CreditAmount.Value);
                        tranDebit += tran.Where(i => i.DebitLedgerAccountId == lItem.Id).Sum(i => i.DebitAmount.Value);
                    }
                }
                catch (Exception ex)
                {
                    Helper.WriteError(ex, ex.Message);
                }

                decimal total = (tranCredit - tranDebit);

                var CheckInner = catTre.FirstOrDefault(j => j.ParentId == inner.Id);
                if (CheckInner != null)
                {
                    amountCount(catTre, CheckInner, inner.Id, tran);
                }
                amt += total;
            }
            return amt;
        }

        #endregion



        [HttpGet]
        public ActionResult Profit()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");

            List<ProfitDto> listStock = new List<ProfitDto>();
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            var sAlert1 = db.ProductStock.Where(i => (i.InventoryTypeId == 2) && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
               .Select(i => new { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, ProfitAmount = i.Profit, AmountWithTax = i.ProfitWithTax}).ToArray();

            var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
            foreach (var item in sAlert2)
            {
                ProfitDto li = new ProfitDto();
                li.ProductName = item;
                li.Quantity = sAlert1.Where(i => i.ProductName == item).Sum(i => i.Quantity);
                li.ProfitAmount = sAlert1.Where(i => i.ProductName == item).Sum(i => i.ProfitAmount);
                li.ProfitAmountWithTax = sAlert1.Where(i => i.ProductName == item).Sum(i => i.AmountWithTax);
                listStock.Add(li);
            }
            ViewBag.company = invoiceFormat.CompanyName;
            return View(listStock);
        }

        [HttpPost]
        public ActionResult Profit(string FromDate, string ToDate, string stime, string etime, int? ProductId = null)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            List<ProfitDto> listStock = new List<ProfitDto>();

            List<ProfitDto> products = new List<ProfitDto>();
            if(ProductId!=null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                products = db.ProductStock.Where(i => (i.InventoryTypeId == 2) && i.ProductId == ProductId && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
               .Select(i => new ProfitDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, ProfitAmount = i.Profit, ProfitAmountWithTax = i.ProfitWithTax }).ToList(); 
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                products = db.ProductStock.Where(i => (i.InventoryTypeId == 2) && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                              .Select(i => new ProfitDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, ProfitAmount = i.Profit, ProfitAmountWithTax = i.ProfitWithTax }).ToList(); 
            }

            var sAlert2 = products.Select(i => i.ProductName).Distinct();
            foreach (var item in sAlert2)
            {
                ProfitDto li = new ProfitDto();
                li.ProductName = item;
                li.Quantity = products.Where(i => i.ProductName == item).Sum(i => i.Quantity);
                li.ProfitAmount = products.Where(i => i.ProductName == item).Sum(i => i.ProfitAmount);
                li.ProfitAmountWithTax = products.Where(i => i.ProductName == item).Sum(i => i.ProfitAmountWithTax);
                listStock.Add(li);
            }

            return View(listStock); 
        }

         
       





        [HttpGet]
        public ActionResult SaleReturn()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");

            SaleDto[] sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 4 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalSaleAmountWithTax, Dated = i.DateAdded.Value,companayname=invoiceFormat.CompanyName }).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }

        [HttpPost]
        public ActionResult SaleReturn(string FromDate, string ToDate, string stime, string etime, int? ProductId = null)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));

            List<SaleDto> sale = new List<SaleDto>();

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 4 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalSaleAmountWithTax, Dated = i.DateAdded.Value }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 4 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalSaleAmountWithTax, Dated = i.DateAdded.Value }).ToList();
            }


            return View(sale);
        }





        [HttpGet]
        public ActionResult PurchaseReturn()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            SaleDto[] Purchase = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 3 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalPurchaseAmount + i.TaxAmount, Dated = i.DateAdded.Value ,companayname=invoiceFormat.CompanyName}).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(Purchase);
        }

        [HttpPost]
        public ActionResult PurchaseReturn(string FromDate, string ToDate, string stime, string etime, int? ProductId = null)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            List<SaleDto> Purchase = new List<SaleDto>();

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                Purchase = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 3 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalPurchaseAmount + i.TaxAmount, Dated = i.DateAdded.Value }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                Purchase = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 3 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalPurchaseAmount + i.TaxAmount, Dated = i.DateAdded.Value }).ToList();
            }


            return View(Purchase);
        }







        [HttpGet]
        public ActionResult Expense()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            ExpenseDto[] expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                .Select(i => new ExpenseDto { Remarks = i.Remarks, Amount = i.Amount,Dated = i.DateAdded,companayname=invoiceFormat.CompanyName}).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(expense);
        }

        [HttpPost]
        public ActionResult Expense(string FromDate, string ToDate, string stime, string etime)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            ExpenseDto[] expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
               .Select(i => new ExpenseDto { Remarks = i.Remarks, Amount = i.Amount, Dated = i.DateAdded }).ToArray();

            return View(expense);
        }


        [HttpGet]
        public ActionResult Due()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            ViewBag.company = invoiceFormat.CompanyName;
            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            DueDto[] due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                .Select(i => new DueDto { Remarks = i.Remarks, Amount = i.DueAmount, Dated = i.DateAdded,IsReturn=i.IsReturn,companayname=invoiceFormat.CompanyName }).ToArray();

            ViewBag.company = invoiceFormat.CompanyName;
            return View(due);
        }

        [HttpPost]
        public ActionResult Due(string FromDate, string ToDate, string stime, string etime, bool? IsReturn)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            List<DueDto> due = new List<DueDto>();
            if(IsReturn==null)
            {
                due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
               .Select(i => new DueDto { Remarks = i.Remarks, Amount = i.DueAmount, Dated = i.DateAdded, IsReturn = i.IsReturn }).ToList();
            }
            else
            {
                due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.IsReturn == IsReturn && i.WarehouseId == warehouse)
               .Select(i => new DueDto { Remarks = i.Remarks, Amount = i.DueAmount, Dated = i.DateAdded, IsReturn = i.IsReturn }).ToList();
            }
             
            return View(due);
        }


        [HttpGet]
        public ActionResult Ladger()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            ViewBag.company = invoiceFormat.CompanyName;
            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            LadgerDto[] ladger = db.Transactions.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                .Select(i => new LadgerDto { Remarks = i.Remarks, Amount = i.CreditAmount, Dated = i.DateAdded, From = i.LedgerAccount_DebitLedgerAccountId.Name, To = i.LedgerAccount_CreditLedgerAccountId.Name,companayname=invoiceFormat.CompanyName }).ToArray();

            ViewBag.company = invoiceFormat.CompanyName;
            return View(ladger);
        }

        [HttpPost]
        public ActionResult Ladger(string FromDate, string ToDate, string stime, string etime)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            LadgerDto[] ladger = db.Transactions.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                .Select(i => new LadgerDto { Remarks = i.Remarks, Amount = i.CreditAmount, Dated = i.DateAdded, From = i.LedgerAccount_DebitLedgerAccountId.Name, To = i.LedgerAccount_CreditLedgerAccountId.Name }).ToArray();

            return View(ladger); 
        }



        [HttpGet]
        public ActionResult DayEnd()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            DayEndDto d = DayEndCombine(Datefrom, Dateto);

            return View(d);
        }
       

        [HttpPost]
        public ActionResult DayEnd(string FromDate, string ToDate, string stime, string etime)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
             
            DayEndDto d = DayEndCombine(Datefrom, Dateto);

            return View(d);
        }
       

        private DayEndDto DayEndCombine(DateTime Datefrom, DateTime Dateto)
        {
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            var expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
            var accoutp = db.AccountPayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
            var due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();

            var ps = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                .Select(i => new { i.TotalSaleAmountWithTax,i.TotalPurchaseAmount,i.TaxAmount,i.Profit,i.ProfitWithTax, i.InventoryTypeId }).ToArray();
            var paymentm = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
            var paytrek = db.Paymenttracks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
            var declared = db.DayEnds.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse);
            var accountsale = db.Sales.Where(i => i.CustomerUserId!= 3 && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();

            DayEndDto d = new DayEndDto();
            d.Sale = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalSaleAmountWithTax);
            d.PurchaseReturn = ps.Where(i => i.InventoryTypeId == 3).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
            d.DueReturn = due.Where(i => i.IsReturn == true).Sum(i => i.DueAmount);
            d.TotalPlus = (d.Sale + d.PurchaseReturn + d.DueReturn);

            d.Expense = expense.Sum(i => i.Amount);
            d.DueGiven = due.Where(i => i.IsReturn == false).Sum(i => i.DueAmount);
            d.SaleReturn = ps.Where(i => i.InventoryTypeId == 4).Sum(i => i.TotalSaleAmountWithTax);
            d.Purchase = ps.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalPurchaseAmount+i.TaxAmount);
            d.stock = ps.Where(i => i.InventoryTypeId == 5).Sum(i => i.TotalPurchaseAmount);
            d.stockplus = ps.Where(i => i.InventoryTypeId == 6).Sum(i => i.TotalPurchaseAmount);
            d.TotalMinus = (d.Expense + d.DueGiven + d.SaleReturn + d.Purchase);

            d.Profit = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.Profit);
            d.ProfitWithTax = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.ProfitWithTax);
            //payment track
            d.rtgs = paytrek.Sum(i => i.swipe)+accoutp.Sum(i=>i.swipe);
             d.ecocash = paytrek.Sum(i => i.ecocash) + accoutp.Sum(i=>i.ecocash);
            d.cashusd = paytrek.Sum(i => i.usd);
            d.telecash = paytrek.Sum(i => i.telecash);
            d.onemoney = paytrek.Sum(i => i.onemoney);
            d.paymentmode = paytrek.Sum(i => i.cash)+accoutp.Sum(i=>i.cash);
            d.tSale = d.rtgs + d.ecocash + d.paymentmode;
            d.accountpayments = accoutp.Sum(i => i.Amount);
            d.accountsales = accountsale.Sum(i => i.TotalAmountWithTax);
            d.Change = paytrek.Sum(i => i.Change) ; /*+accoutp.Sum(i => i.Change)*/
            //payment track


            //declared
            d.cashdeclared = declared.Sum(i => i.totalcash);
            d.ecocashdeclared = declared.Sum(i => i.ecocash);
            d.telecashdeclared = declared.Sum(i => i.telecash);
            d.onemoneydeclared = declared.Sum(i => i.onemoney);
            d.cashusddeclared = declared.Sum(i => i.totalCashUsd);
            d.swipedeclared = declared.Sum(i => i.rtgs);
            d.totaldeclared= declared.Sum(i => i.totalAmount);
            d.accumulatedchange = declared.Sum(i => i.totalChange);
            //declared
            //outages
            d.outagecash = d.paymentmode - d.cashdeclared;
            d.outageecocash = d.ecocash - d.ecocashdeclared;
            d.outagetelecash = d.telecash - d.telecashdeclared;
            d.outageonemoney = d.onemoney - d.onemoneydeclared;
            d.outagecashusd = d.cashusd - d.cashusddeclared;
            d.outageswipe = d.rtgs - d.swipedeclared;
            d.outageChange = d.Change - d.cashdeclared;
            d.outagetotal = d.tSale - (d.cashdeclared + d.ecocashdeclared + d.swipedeclared);


            //outage
         

            d.companayname = invoiceFormat.CompanyName;
            return d;
        }




        //Day end report with declarations
        [HttpGet]
        public ActionResult EndDay()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            DayEndDto d = DayEndDeclare(Datefrom, Dateto);

            return View(d);
        }
        // Purchases Report
        [HttpGet]
        public ActionResult Purchase()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            ViewBag.company = invoiceFormat.CompanyName;
            PurchaseDto[] Purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.WarehouseId == warehouse)
                .Select(i => new PurchaseDto { ProductName = i.Product_ProductId.Name, supplierId = i.User_VendorUserId.UserName, Quantity = i.Quantity, TotalPuchaseAmount = i.TotalAmount, Dated = i.DateAdded,companayname=invoiceFormat.CompanyName }).ToArray();
            PurchaseDto d = new PurchaseDto();
            d.companayname = invoiceFormat.CompanyName;
            return View(Purchase);
        }

        [HttpPost]
        public ActionResult Purchase(string FromDate, string ToDate, string stime, string etime, int? ProductId = null)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));

            var ph = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray()
               .Select(i => new { i.Product_ProductId.Name, i.Quantity, i.TotalAmount, i.DateAdded, i.VendorUserId }).ToArray();

            var ps = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.InventoryTypeId, i.CGST }).ToArray();
      
            var vendor = db.Users.Where(i => i.RoleId == 3)
                .Select(i => new { i.UserName, i.Id }).ToArray();
           
            List<PurchaseDto> Purchase = new List<PurchaseDto>();

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
               // foreach (var ven in vendor)
              //  {



                    Purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                    .Select(i => new PurchaseDto { ProductName = i.Product_ProductId.Name, supplierId=i.User_VendorUserId.UserName, Quantity = i.Quantity, TotalPuchaseAmount = i.TotalAmount, Dated = i.DateAdded }).ToList()
                  
                    ;
                }
        //    }
            else
            {
              //  foreach (var ven in vendor)
              //  {

                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                    Purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.WarehouseId == warehouse)
                    .Select(i => new PurchaseDto { ProductName = i.Product_ProductId.Name, supplierId = i.User_VendorUserId.UserName, Quantity = i.Quantity, TotalPuchaseAmount = i.TotalAmount, Dated = i.DateAdded }).ToList();
              //  }
            }


            return View(Purchase);
        }
        
        //VAT report
        [HttpGet]
        public ActionResult VAT()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            //retrunDate = Convert.ToDateTime(finalDate.ToString("d/M/yyyy"));
            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            VATReport d = VAT(Datefrom, Dateto);

            return View(d);
        }
        [HttpPost]
        public ActionResult VAT(string FromDate,string ToDate, string stime, string etime)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(string.Format(FromDate,"d/M/yyyy")), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(string.Format(ToDate, "d/M/yyyy")), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);

            VATReport d = VAT(Datefrom, Dateto);

            return View(d);
        }
        private VATReport VAT(DateTime Datefrom, DateTime Dateto)
        {
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            var purchasess = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();

            var ps = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse && i.IsFormal == true)
                .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.InventoryTypeId,i.CGST }).ToArray();
           

            VATReport d = new VATReport();
            d.Sale = ps.Where(i=>i.InventoryTypeId==2).Sum(i => i.TotalSaleAmountWithTax);
            d.salesexcludevat = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalSaleAmountWithTax - i.TaxAmount);
            d.taxablesales = ps.Where(i =>i.CGST == 2 && i.InventoryTypeId == 2 ).Sum(i => i.TotalSaleAmountWithTax - i.TaxAmount);
            d.nontaxablesales = ps.Where(i => i.InventoryTypeId == 2 && i.CGST == 5).Sum(i => i.TotalSaleAmountWithTax);
            d.SaleReturn= ps.Where(i => i.InventoryTypeId == 4).Sum(i => i.TotalSaleAmountWithTax);
            d.totalPurchase= ps.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalPurchaseAmount);
            d.taxablepurchases = ps.Where(i => i.InventoryTypeId == 1 && i.CGST==2).Sum(i => i.TotalPurchaseAmount-i.TaxAmount);
           d.nontaxablepurchase= ps.Where(i => i.InventoryTypeId == 2 && i.CGST == 5).Sum(i => i.TotalPurchaseAmount);
            d.Totapurchaseexcludingvat = ps.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalPurchaseAmount-i.TaxAmount);
            d.taxonpurchases = ps.Where(i => i.InventoryTypeId == 1).Sum(i => i.TaxAmount);
            d.taxonsales = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.TaxAmount);
            d.nettax = d.taxonsales - d.taxonpurchases;
            d.datefrom = Datefrom;
            d.dateto = Dateto;
            d.companayname = invoiceFormat.CompanyName;

            return d;
        }

        [HttpGet]
        public ActionResult DeclareDayEnd()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            DayEndDto d = DayEndDeclare(Datefrom, Dateto);

            return View(d);
        }


        [HttpPost]
        public ActionResult DeclareDayEnd(string FromDate, string ToDate, string stime, string etime)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);

            DayEndDto d = DayEndDeclare(Datefrom, Dateto);

            return View(d);
        }
        private DayEndDto DayEndDeclare(DateTime Datefrom, DateTime Dateto)
        {
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
    
            var expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();

            var due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();

            var ps = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.Profit, i.ProfitWithTax, i.InventoryTypeId }).ToArray();
            var paymentm = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)  && i.WarehouseId == warehouse).ToArray();
            var declared = db.DayEnds.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse);

            DayEndDto d = new DayEndDto();
            d.Sale = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalSaleAmountWithTax);
            d.PurchaseReturn = ps.Where(i => i.InventoryTypeId == 3).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
            d.DueReturn = due.Where(i => i.IsReturn == true).Sum(i => i.DueAmount);
            d.TotalPlus = (d.Sale + d.PurchaseReturn + d.DueReturn);

            d.Expense = expense.Sum(i => i.Amount);
            d.DueGiven = due.Where(i => i.IsReturn == false).Sum(i => i.DueAmount);
            d.SaleReturn = ps.Where(i => i.InventoryTypeId == 4).Sum(i => i.TotalSaleAmountWithTax);
            d.Purchase = ps.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);

            d.TotalMinus = (d.Expense + d.DueGiven + d.SaleReturn + d.Purchase);

            d.Profit = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.Profit);
            d.ProfitWithTax = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.ProfitWithTax);
          
            d.telecash = paymentm.Where(i => i.PaymentModeId == 3).Sum(i => i.PaidAmount);
            d.onemoney = paymentm.Where(i => i.PaymentModeId == 0).Sum(i => i.PaidAmount);
            d.cashusd = paymentm.Where(i => i.PaymentModeId == 6).Sum(i => i.PaidAmount);
            d.fca = paymentm.Where(i => i.PaymentModeId == 6).Sum(i => i.PaidAmount);
            d.nostro = paymentm.Where(i => i.PaymentModeId == 7).Sum(i => i.PaidAmount);
            d.paymentmode = paymentm.Where(i => i.PaymentModeId == 1).Sum(i => i.PaidAmount);
           
            //declared
            d.cashdeclared = declared.Select(i => i.totalcash).FirstOrDefault();
            d.cashusd = declared.Select(i => i.totalCashUsd).FirstOrDefault();
            d.ecocashdeclared = declared.Select(i => i.ecocash).FirstOrDefault();
            d.swipedeclared = declared.Select(i => i.rtgs).FirstOrDefault();
            d.totaldeclared = declared.Select(i => i.totalAmount).FirstOrDefault();
            //declared
            //old
            d.telecash = paymentm.Where(i => i.PaymentModeId == 3).Sum(i => i.PaidAmount);
            d.cashold = paymentm.Where(i => i.PaymentModeId == 1).Sum(i => i.PaidAmount);
            d.ecocashold = paymentm.Where(i => i.PaymentModeId == 4).Sum(i => i.PaidAmount);
            d.swipeold = paymentm.Where(i => i.PaymentModeId == 2).Sum(i => i.PaidAmount);
            d.onemoney = paymentm.Where(i => i.PaymentModeId == 5).Sum(i => i.PaidAmount);
            d.cashusd = paymentm.Where(i => i.PaymentModeId == 6).Sum(i => i.PaidAmount);
            d.fca = paymentm.Where(i => i.PaymentModeId == 7).Sum(i => i.PaidAmount);
            d.nostro = paymentm.Where(i => i.PaymentModeId == 1006).Sum(i => i.PaidAmount);
            //old
            //outage old
            d.outagecasho =  d.cashdeclared-d.cashold ;
            d.outageecocasho =   d.ecocashdeclared-d.ecocashold;
            d.outageswipeo =  d.swipedeclared-d.swipeold ;
            d.outagetotalo = d.outageswipeo + d.outageecocasho + d.outagecasho;
            //outage old
            return d;
        }
       

        private void BaseOfReport(string stime, string etime, DateTime Datefrom, DateTime Dateto,int IsGet)
        {
            ViewBag.FromDate = Datefrom.ToString("dd/MMM/yyyy");
            ViewBag.ToDate = Datefrom.ToString("dd/MMM/yyyy");

            ViewBag.stime = stime;
            ViewBag.etime = etime;
            ViewBag.timess = IsGet;

            try
            {
                ViewBag.start = Datefrom;
                ViewBag.end = Dateto;
            }
            catch (Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
            }
        }
         
	}
}
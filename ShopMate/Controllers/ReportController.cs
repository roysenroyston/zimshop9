using ShopMate.ModelDto;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class ReportController : Controller
    {
        private SIContext db = new SIContext();
        int warehouses = int.Parse(Env.GetUserInfo("WarehouseId"));
        //
        [HttpGet]
        public ActionResult cashierreport()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

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
                li.totalsalesfortheday = sAlert1.Where(i => i.Tilloperator == item).Sum(i => i.totalsalesfortheday);
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
        //[HttpGet]
        //public ActionResult ExpiryAlert()
        //{
        //    var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
        //    var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

        //    BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

        //    List<ExpiryAlertDto> listStock = new List<ExpiryAlertDto>();// Do we have a product here?yes now m thinkin i did get expiry date i can get the rest of the infor that way ndoisa futi batch number . Ok let me try that
        //    int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
        //    var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
        //    var sAlert1 = db.ProductStock.Where(i => (i.InventoryTypeId == 1 || i.InventoryTypeId == 2 || i.InventoryTypeId == 5 || i.InventoryTypeId == 6 || i.InventoryTypeId == 7) && i.WarehouseId == warehouse /*&& i.ProductBatchId > 0 && i.ProductBatch_ProductBatchId.IsCleared == false*/)
        //       .Select(i => new { ProductName = i.Product_ProductId.Name, InvTypeId = i.InventoryTypeId, ExpiryAlert = i.Product_ProductId.StockAlert, /*BatchNumberId = i.ProductBatch_ProductBatchId.BatchNumber, ExpiryDate = i.ProductBatch_ProductBatchId.ExpiryDate*/ }).ToArray();

        //    var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
        //    foreach (var item in sAlert2)
        //    {
        //        ExpiryAlertDto li = new ExpiryAlertDto();
        //        var today = DateTime.Now;
        //        var ExpiryDate = sAlert1.FirstOrDefault(i => i.ProductName == item).ExpiryDate;
        //        var PeriodToExpiry = (today - sAlert1.FirstOrDefault(i => i.ProductName == item).ExpiryDate).Days;
        //        var itemBatchNumberIds = sAlert1.FirstOrDefault(i => i.ProductName == item).BatchNumberId;
        //        var ExpiryAlert = sAlert1.FirstOrDefault(i => i.ProductName == item).ExpiryAlert;
        //        if (PeriodToExpiry <= ExpiryAlert)
        //        {
        //            var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);
        //            li.ProductName = item;
        //            li.PeriodToExpiry = PeriodToExpiry;
        //            li.BatchNumber = sAlert1.FirstOrDefault(i => i.ProductName == item).BatchNumberId;
        //            //li.Quantity = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Quantity) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Quantity)- sAlert1.Where(i => i.InvTypeId == 7 && i.ProductName == item).Sum(i => i.Quantity) ;
        //            //li.StockAlert = selectedProduct.StockAlert;
        //            // li.StockAlert = sAlert1.FirstOrDefault(i => i.InvTypeId == 1 && i.ProductName == item).StockAlert;
        //            listStock.Add(li);
        //        }
        //    }
        //    ViewBag.company = invoiceFormat.CompanyName;
        //    return View(listStock);
        //}


        [HttpPost]
        public ActionResult cashierreport(string FromDate, string ToDate, string stime, string etime)
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
                li.totalsalesfortheday = sAlert1.Where(i => i.Tilloperator == item).Sum(i => i.totalsalesfortheday) + accountsale.Where(i => i.Tilloperator == item).Sum(i => i.accountsales); ;
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

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name");
            if (warehouse == 1)
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(m => m.RoleId == 2), "Id", "UserName");
            }
            else
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(m => m.Id == warehouse), "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(m => m.WarehouseId == warehouse && m.RoleId == 2), "Id", "UserName");
            }

            List<SaleDto> sale = new List<SaleDto>();

            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
   
       
                sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse)
                  .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, SalePrice = i.SalePrice, RtgsPrice = i.RtgsPrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, TotalRtgs = i.TotalRtgsAmount,/*InvoiceId = i.InvoiceId,*/AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, Dated = i.DateAdded.Value }).ToList();
            
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }
        [HttpGet]
        public ActionResult ProductHistory()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name");
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            if (warehouse == 1)
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(m => m.RoleId == 2), "Id", "UserName");
            }
            else
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(m => m.Id == warehouse), "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(m => m.WarehouseId == warehouse && m.RoleId == 2), "Id", "UserName");
            }

            List<ProductHistDto> sale = new List<ProductHistDto>();
            if (warehouse != 1)
            {
                ViewBag.ProductId = new SelectList(db.Products.Where(i => i.WarehouseId == warehouse), "Id", "Name");
                sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                .Select(i => new ProductHistDto { ProductName = i.Product_ProductId.Name, AddedBy = (db.Users.FirstOrDefault(j => j.Id == i.AddedBy).UserName), RemainingQuantity = i.RemainingQuantity, Quantity = i.Quantity, PurchasePrice = i.PurchasePrice, SalePrice = i.SalePrice, TotalSaleAmount = i.TotalSaleAmount, InventoryTypeId = i.InventoryType_InventoryTypeId.Name, TotalPurchaseAmount = i.TotalPurchaseAmount, DateAdded = i.DateAdded, }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                .Select(i => new ProductHistDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, AddedBy = (db.Users.FirstOrDefault(j => j.Id == i.AddedBy).UserName), PurchasePrice = i.PurchasePrice, SalePrice = i.SalePrice, RemainingQuantity = i.RemainingQuantity, TotalPurchaseAmount = i.TotalPurchaseAmount, InventoryTypeId = i.InventoryType_InventoryTypeId.Name, TotalSaleAmount = i.TotalSaleAmount, DateAdded = i.DateAdded }).ToList();
            }
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }

        [HttpGet]
        public ActionResult Discount()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name");
            if (warehouse == 1)
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(m => m.RoleId == 2), "Id", "UserName");
            }
            else
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(m => m.Id == warehouse), "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(m => m.WarehouseId == warehouse && m.RoleId == 2), "Id", "UserName");
            }

            List<SaleDto> sale = new List<SaleDto>();

            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            if ((db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name) == "")
            {
                sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2)
                   .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, SalePrice = i.SalePrice, RtgsPrice = i.RtgsPrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, TotalRtgs = i.TotalRtgsAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, Dated = i.DateAdded.Value }).ToList();
            }
            else
            {
                sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse)
                  .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, SalePrice = i.SalePrice, RtgsPrice = i.RtgsPrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, TotalRtgs = i.TotalRtgsAmount,/*InvoiceId = i.InvoiceId,*/AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, Dated = i.DateAdded.Value }).ToList();
            }
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }
        [HttpPost]
        public ActionResult Discount(string FromDate, string ToDate, string stime, string etime, int? ProductId = null, int? UserId = null)
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
            var myWareId = int.Parse(Env.GetUserInfo("WarehouseId")); /*db.Users.FirstOrDefault(i => i.Id == UserId).WarehouseId;*/
            //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            int warehouse = (int)myWareId;
            //BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            var WarehouseId = Env.GetUserInfo("WarehouseId");

            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            List<SaleDto> sale = new List<SaleDto>();
            if (WarehouseId == null)
            {
                if (WarehouseId == "1")
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                    ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                }
                else
                {
                    ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name", ProductId);
                    ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(n => n.Id == warehouse), "Id", "Name");
                }

            }
            else
            {
                warehouse = (int)myWareId;
            }
            if ((db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name) == "")
            {
                //if (IsFormalId == null || db.InvoiceTypes.FirstOrDefault(i => i.Id == (IsFormalId)).Name == "ALL")
                //{


                if (ProductId != null)
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                    ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                    ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                    sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, userid = UserId, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                }
                else
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                    ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                    ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                    sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, SalePrice = i.SalePrice, RtgsPrice = i.RtgsPrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, userid = UserId, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                }

            }

            else
            {

                if (ProductId != null)
                {
                    if (warehouse != 1)
                    {
                        ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name", ProductId);
                        ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                        ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(n => n.Id == warehouse), "Id", "Name");
                        ViewBag.UserId = new SelectList(db.Users.Where(m => m.WarehouseId == warehouse && m.RoleId == 2), "Id", "UserName");
                    }
                    else
                    {
                        ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                        ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                        ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                        ViewBag.UserId = new SelectList(db.Users.Where(m => m.RoleId == 2), "Id", "UserName");
                    }

                    sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                        .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, Amount = i.TotalAmount, userid = UserId, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                }
                else
                {
                    if (warehouse != 1)
                    {
                        ViewBag.ProductId = new SelectList(db.Products.Where(i => i.WarehouseId == warehouse), "Id", "Name");
                        ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                        ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(n => n.Id == warehouse), "Id", "Name");
                        ViewBag.UserId = new SelectList(db.Users.Where(m => m.WarehouseId == warehouse && m.RoleId == 2), "Id", "UserName");

                    }
                    else
                    {
                        ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                        ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                        ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                        ViewBag.UserId = new SelectList(db.Users.Where(m => m.RoleId == 2), "Id", "UserName");

                    }

                    sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse && i.AddedBy == UserId)
                         .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                }

            }

            return View(sale.OrderBy(i => i.Dated));

        }
        [HttpGet]
        public ActionResult VanSaleReport()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            VanSaleItemDto[] sale = db.VanSaleItems.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                .Select(i => new VanSaleItemDto { ClosingStock = (int)i.ClosingStock, DateAdded = i.DateAdded, SalePrice = i.SalePrice, GoodsSold = i.GoodsSold, GP = i.GP, OverallGP = (decimal)i.OverallGP, Id = i.Id, OpeningStock = i.OpeningStock, Sales = i.Sales, StockValue = i.StockValue, StockAmount = i.StockAmount, UnitPrice = i.UnitPrice, VanSaleId = i.VanSaleId, Product = i.Product_ProductId.Name, Route = i.VanSale_VanSaleId.Route }).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }

        [HttpPost]
        public ActionResult VanSaleReport(string FromDate, string ToDate, string stime, string etime, int? ProductId = null, int? IsFormalId = null)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 1);

            //var text = db.InvoiceTypes.FirstOrDefault(i => i.Id == (IsFormalId)).Name;
            //var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            //var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);
            var myDate = FromDate + " " + stime;
            var yourDate = ToDate + " " + etime;
            CultureInfo provider = CultureInfo.InvariantCulture;
            //DateTime Datefrom; // 1/1/0001 12:00:00 AM  
            //DateTime Dateto; // 1/1/0001 12:00:00 AM 
            //bool isSuccess4 = DateTime.TryParseExact(myDate, "MM-dd-yyyy HH:mm", provider, DateTimeStyles.None, out Datefrom);
            //bool isSuccess3 = DateTime.TryParseExact(yourDate, "MM-dd-yyyy HH:mm", provider, DateTimeStyles.None, out Dateto);


            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            List<VanSaleItemDto> sale = new List<VanSaleItemDto>();



            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);


                sale = db.VanSaleItems.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
               .Select(i => new VanSaleItemDto { ClosingStock = (int)i.ClosingStock, DateAdded = i.DateAdded, SalePrice = i.SalePrice, GoodsSold = i.GoodsSold, GP = i.GP, OverallGP = (decimal)i.OverallGP, Id = i.Id, OpeningStock = i.OpeningStock, Sales = i.Sales, StockValue = i.StockValue, StockAmount = i.StockAmount, UnitPrice = i.UnitPrice, VanSaleId = i.VanSaleId, Product = i.Product_ProductId.Name, Route = i.VanSale_VanSaleId.Route }).ToList();


            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");

                sale = db.VanSaleItems.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
             .Select(i => new VanSaleItemDto { ClosingStock = (int)i.ClosingStock, DateAdded = i.DateAdded, SalePrice = i.SalePrice, GoodsSold = i.GoodsSold, GP = i.GP, OverallGP = (decimal)i.OverallGP, Id = i.Id, OpeningStock = i.OpeningStock, Sales = i.Sales, StockValue = i.StockValue, StockAmount = i.StockAmount, UnitPrice = i.UnitPrice, VanSaleId = i.VanSaleId, Product = i.Product_ProductId.Name, Route = i.VanSale_VanSaleId.Route }).ToList();

            }


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
                .Select(i => new GrvDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.UnitPrice, Amount = i.TotalAmount, Supplier = i.Vendor_Id.UserName, Dated = i.DateAdded }).ToArray();
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
                .Select(i => new GrvDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.UnitPrice, Amount = i.TotalAmount, Supplier = i.Vendor_Id.UserName, Dated = i.DateAdded }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                Purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.WarehouseId == warehouse)
                .Select(i => new GrvDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.UnitPrice, Amount = i.TotalAmount, Supplier = i.Vendor_Id.UserName, Dated = i.DateAdded }).ToList();
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
            ShrinkageDto[] sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && (i.InventoryTypeId == 5 || i.InventoryTypeId == 6 || i.InventoryTypeId == 7) && i.WarehouseId == warehouse)
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
            ManufacturingDto[] sale = db.FinishedItems.Where(i => (i.dateadded >= Datefrom && i.dateadded <= Dateto) && i.WarehouseId == warehouse)
                .Select(i => new ManufacturingDto { Name = i.Product_ProductId.Name, Quantity = i.Quantity, unitprice = i.unitprice, Total = i.Total, Dated = i.dateadded, warehouseid = i.WarehouseId }).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }
        [HttpPost]
        public ActionResult Manufacturing(string FromDate, string ToDate, string stime, string etime, int? ProductId = null)
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
                sale = db.FinishedItems.Where(i => (i.dateadded >= Datefrom && i.dateadded <= Dateto) && i.WarehouseId == warehouse)
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
        public ActionResult TodaySale(string FromDate, string ToDate, string stime, string etime, string IsFormalId, int? ProductId = null, int? UserId = null)
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

            var myWareId = int.Parse(Env.GetUserInfo("WarehouseId"));//db.Users.FirstOrDefault(i => i.Id == UserId).WarehouseId;


            //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            int warehouse = (int)myWareId;
            //BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            var WarehouseId = Env.GetUserInfo("WarehouseId");

            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            List<SaleDto> sale = new List<SaleDto>();
            if (WarehouseId == null)
            {
                if (WarehouseId == "1")
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                    ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                }
                else
                {
                    ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name", ProductId);
                    ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(n => n.Id == warehouse), "Id", "Name");
                }

            }
            else
            {
                warehouse = (int)myWareId;
            }
            if ((db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name) == "")
            {
                //if (IsFormalId == null || db.InvoiceTypes.FirstOrDefault(i => i.Id == (IsFormalId)).Name == "ALL")
                //{
                if (IsFormalId == "ZWG"){
                    if (ProductId != null)
                    {
                        ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                        ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                        ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                        sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.PaymentModeId != 6)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, userid = UserId, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                    }
                    else
                    {
                        ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                        ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                        ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                        sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.PaymentModeId != 6)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, SalePrice = i.SalePrice, RtgsPrice = i.RtgsPrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, userid = UserId, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                    }

                }
                else if (IsFormalId =="USD")  {
                    if (ProductId != null)
                    {
                        ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                        ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                        ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                        sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.PaymentModeId == 6)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, userid = UserId, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                    }
                    else
                    {
                        ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                        ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                        ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                        sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.PaymentModeId == 6)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, SalePrice = i.SalePrice, RtgsPrice = i.RtgsPrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, userid = UserId, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                    }
                } 
                else {
                    if (ProductId != null)
                    {
                        ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                        ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                        ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                        sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, userid = UserId, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                    }
                    else
                    {
                        ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                        ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                        ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                        sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2)
                    .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, SalePrice = i.SalePrice, RtgsPrice = i.RtgsPrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, userid = UserId, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                    }

                }

          

            }

            else
            {
                ViewBag.ProductId = new SelectList(db.Products.Where(i => i.WarehouseId == warehouse), "Id", "Name");
                ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(n => n.Id == warehouse), "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(m => m.WarehouseId == warehouse && m.RoleId == 2), "Id", "UserName");

                if (IsFormalId =="USD") {
                    if (ProductId != null)
                    {

                        if (UserId == null)
                        {
                            sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse && i.PaymentModeId == 6)
                          .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, Amount = i.TotalAmount, userid = UserId, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                        }
                        else
                        {
                            sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse && i.AddedBy == UserId && i.PaymentModeId == 6)
                          .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, Amount = i.TotalAmount, userid = UserId, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                        }

                    }
                    else
                    {

                        if (UserId == null)
                        {
                            sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse && i.PaymentModeId == 6)
                                 .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                        }
                        else
                        {
                            sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse && i.AddedBy == UserId && i.PaymentModeId == 6)
                                       .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                        }

                    }

                }
                else if (IsFormalId == "ZWG") {
                    if (ProductId != null)
                    {

                        if (UserId == null)
                        {
                            sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse && i.PaymentModeId != 6)
                          .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, Amount = i.TotalAmount, userid = UserId, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                        }
                        else
                        {
                            sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse && i.AddedBy == UserId && i.PaymentModeId != 6)
                          .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, Amount = i.TotalAmount, userid = UserId, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                        }

                    }
                    else
                    {

                        if (UserId == null)
                        {
                            sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse && i.PaymentModeId != 6)
                                 .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                        }
                        else
                        {
                            sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse && i.AddedBy == UserId && i.PaymentModeId != 6)
                                       .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                        }

                    }

                }
                else {
                    if (ProductId != null)
                    {

                        if (UserId == null)
                        {
                            sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                          .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, Amount = i.TotalAmount, userid = UserId, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                        }
                        else
                        {
                            sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.ProductId == ProductId && i.WarehouseId == warehouse && i.AddedBy == UserId)
                          .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, Amount = i.TotalAmount, userid = UserId, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                        }

                    }
                    else
                    {

                        if (UserId == null)
                        {
                            sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse)
                                 .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                        }
                        else
                        {
                            sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse && i.AddedBy == UserId)
                                       .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, CustomerName = i.CustomerName, UnitPrice = i.UnitPrice, TotalRtgs = i.TotalRtgsAmount, RecieptNumber = i.recieptNumber.ToString(), Quantity = i.Quantity, RtgsPrice = i.RtgsPrice, SalePrice = i.SalePrice, Amount = i.TotalAmount, AmountDiscounted = i.discount, AmountPaid = i.PaidAmount, WithTaxAmount = i.TotalAmountWithTax, /*InvoiceId = i.InvoiceId,*/ Dated = i.DateAdded.Value }).ToList();
                        }

                    }

                }

            }

            return View(sale.OrderBy(i => i.Dated));

        }

        [HttpPost]
        public ActionResult ProductHistory(string FromDate, string ToDate, string stime, string etime, int? ProductId = null)
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

            var myWareId = int.Parse(Env.GetUserInfo("WarehouseId"));//db.Users.FirstOrDefault(i => i.Id == UserId).WarehouseId;


            //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            int warehouse = (int)myWareId;
            //BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            var WarehouseId = Env.GetUserInfo("WarehouseId");

            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            //     List<SaleDto> sale = new List<SaleDto>();

            if (WarehouseId == "1")
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name", ProductId);
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(n => n.Id == warehouse), "Id", "Name");
            }

            List<ProductHistDto> sale = new List<ProductHistDto>();
            if (ProductId == null)
            {
                if (warehouse != 1)
                {
                    ViewBag.ProductId = new SelectList(db.Products.Where(i => i.WarehouseId == warehouse), "Id", "Name");
                    sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                    .Select(i => new ProductHistDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, PurchasePrice = i.PurchasePrice, SalePrice = i.SalePrice, TotalSaleAmount = i.TotalSaleAmount, RemainingQuantity = i.RemainingQuantity, InventoryTypeId = i.InventoryType_InventoryTypeId.Name, AddedBy = (db.Users.FirstOrDefault(j => j.Id == i.AddedBy).UserName), TotalPurchaseAmount = i.TotalPurchaseAmount, DateAdded = i.DateAdded, }).ToList();
                }
                else
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                    sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                    .Select(i => new ProductHistDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, PurchasePrice = i.PurchasePrice, SalePrice = i.SalePrice, TotalPurchaseAmount = i.TotalPurchaseAmount, RemainingQuantity = i.RemainingQuantity, InventoryTypeId = i.InventoryType_InventoryTypeId.Name, AddedBy = (db.Users.FirstOrDefault(j => j.Id == i.AddedBy).UserName), TotalSaleAmount = i.TotalSaleAmount, DateAdded = i.DateAdded }).ToList();
                }
            }
            else
            {
                if (warehouse != 1)
                {
                    ViewBag.ProductId = new SelectList(db.Products.Where(i => i.WarehouseId == warehouse), "Id", "Name", ProductId);
                    sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse && i.ProductId == ProductId)
                    .Select(i => new ProductHistDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, PurchasePrice = i.PurchasePrice, SalePrice = i.SalePrice, TotalSaleAmount = i.TotalSaleAmount, RemainingQuantity = i.RemainingQuantity, InventoryTypeId = i.InventoryType_InventoryTypeId.Name, TotalPurchaseAmount = i.TotalPurchaseAmount, AddedBy = (db.Users.FirstOrDefault(j => j.Id == i.AddedBy).UserName), DateAdded = i.DateAdded, }).ToList();
                }
                else
                {
                    ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                    sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.ProductId == ProductId)
                    .Select(i => new ProductHistDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, PurchasePrice = i.PurchasePrice, SalePrice = i.SalePrice, TotalPurchaseAmount = i.TotalPurchaseAmount, RemainingQuantity = i.RemainingQuantity, InventoryTypeId = i.InventoryType_InventoryTypeId.Name, AddedBy = (db.Users.FirstOrDefault(j => j.Id == i.AddedBy).UserName), TotalSaleAmount = i.TotalSaleAmount, DateAdded = i.DateAdded }).ToList();
                }
            }


            //  return View(sale);



            return View(sale.OrderBy(i => i.DateAdded));

        }
        [HttpPost]
        public ActionResult formalreport(string FromDate, string ToDate, string stime, string etime, string IsFormalId, int? ProductId = null)
        {
            //ViewBag.IsFormalId = new SelectList(db.Products, "Id", "Name", ProductId);
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);



            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            List<ProductHistDto> sale = new List<ProductHistDto>();




            if (warehouse != 1)
            {
                ViewBag.ProductId = new SelectList(db.Products.Where(i => i.WarehouseId == warehouse), "Id", "Name", ProductId);
                sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                .Select(i => new ProductHistDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, TotalSaleAmount = i.TotalSaleAmount, TotalPurchaseAmount = i.TotalPurchaseAmount, DateAdded = i.DateAdded, }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                .Select(i => new ProductHistDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.PurchasePrice, TotalPurchaseAmount = i.TotalPurchaseAmount, TotalSaleAmount = i.TotalSaleAmount, DateAdded = i.DateAdded }).ToList();
            }

            return View(sale);


        }


        [HttpGet]
        public ActionResult TodayPurchase()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.Products.Where(m => m.WarehouseId == warehouse), "Id", "Name");

            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            SaleDto[] Purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, TaxAmount=i.TaxAmount, Quantity = i.Quantity, SalePrice = i.UnitPrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded, InvoiceNumber = i.InvoiceNumber }).ToArray();
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
                ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name", ProductId);
                Purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name,TaxAmount= i.TaxAmount, Quantity = i.Quantity, SalePrice = i.UnitPrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded, InvoiceNumber = i.InvoiceNumber }).ToList();

            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name");
                Purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, TaxAmount = i.TaxAmount, Quantity = i.Quantity, SalePrice = i.UnitPrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded, InvoiceNumber = i.InvoiceNumber }).ToList();

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
            // var sAlert1 = db.ProductStock.Where(i => (i.InventoryTypeId == 1 || i.InventoryTypeId == 2 ||i.InventoryTypeId==5 || i.InventoryTypeId==6 ||i.InventoryTypeId==7) && i.WarehouseId == warehouse)
            //  .Select(i => new { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity,InvTypeId=i.InventoryTypeId,StockAlert=i.Product_ProductId.StockAlert}).ToArray();

            var sAlert1 = db.WarehouseStocks.Where(m => m.WarehouseId == warehouse)
      .Select(o => new { ProductName = o.Product_ProductId.Name, Quantity = o.RemainingQuantity, StockAlert = o.Product_ProductId.StockAlert }).ToArray();

            var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
            foreach (var item in sAlert2)
            {
                StockAlertDto li = new StockAlertDto();
                var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);
                li.ProductName = item;
                li.Quantity = db.WarehouseStocks.FirstOrDefault(i => i.Product_ProductId.Name == item).RemainingQuantity;
                //li.Quantity = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Quantity) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Quantity)- sAlert1.Where(i => i.InvTypeId == 7 && i.ProductName == item).Sum(i => i.Quantity) ;
                li.StockAlert = selectedProduct.StockAlert;
                // li.StockAlert = sAlert1.FirstOrDefault(i => i.InvTypeId == 1 && i.ProductName == item).StockAlert;
                listStock.Add(li);
            }
            ViewBag.company = invoiceFormat.CompanyName;
            return View(listStock);
        }
        // [HttpGet]
        //public ActionResult ExpiryAlert()
        //{
        //    var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
        //    var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

        //    BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

        //    List<ExpiryAlertDto> listStock = new List<ExpiryAlertDto>();// Do we have a product here?yes now m thinkin i did get expiry date i can get the rest of the infor that way ndoisa futi batch number . Ok let me try that
        //    int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
        //    var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
        //    var sAlert1 = db.ProductStock.Where(i => (i.InventoryTypeId == 1 || i.InventoryTypeId == 2 || i.InventoryTypeId == 5 || i.InventoryTypeId == 6 || i.InventoryTypeId == 7) && i.WarehouseId == warehouse && i.ProductBatchId > 0 && i.ProductBatch_ProductBatchId.IsCleared == false)
        //       .Select(i => new { ProductName = i.Product_ProductId.Name, InvTypeId = i.InventoryTypeId,  BatchNumberId = i.ProductBatch_ProductBatchId.BatchNumber, ExpiryDate = i.ProductBatch_ProductBatchId.ExpiryDate }).ToArray();

        //    var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
        //    foreach (var item in sAlert2)
        //    {
        //        ExpiryAlertDto li = new ExpiryAlertDto();
        //        var today = DateTime.Now;
        //        var ExpiryDate = sAlert1.FirstOrDefault(i => i.ProductName == item).ExpiryDate;
        //        var PeriodToExpiry = (today - sAlert1.FirstOrDefault(i => i.ProductName == item).ExpiryDate).Days;
        //        var itemBatchNumberIds = sAlert1.FirstOrDefault(i => i.ProductName == item).BatchNumberId;
        //        var ExpiryAlert = sAlert1.FirstOrDefault(i => i.ProductName == item);
        //        if (PeriodToExpiry <= ExpiryAlert) { 
        //        var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);
        //        li.ProductName = item;
        //        li.PeriodToExpiry = PeriodToExpiry;
        //        li.BatchNumber = sAlert1.FirstOrDefault(i => i.ProductName == item).BatchNumberId;
        //        //li.Quantity = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Quantity) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Quantity)- sAlert1.Where(i => i.InvTypeId == 7 && i.ProductName == item).Sum(i => i.Quantity) ;
        //        //li.StockAlert = selectedProduct.StockAlert;
        //        // li.StockAlert = sAlert1.FirstOrDefault(i => i.InvTypeId == 1 && i.ProductName == item).StockAlert;
        //        listStock.Add(li);
        //        }
        //    }
        //    ViewBag.company = invoiceFormat.CompanyName;
        //    return View(listStock);
        //}


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

            var sAlert1 = db.Products.Where(i => i.WarehouseId == warehouse)
               .Select(i => new { ProductName = i.Name, Quantity = i.RemainingQuantity, cost = i.RemainingQuantity * i.PurchasePrice, Amount = i.RemainingQuantity * i.SalePrice }).ToArray();
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

            var sAlert1 = db.Products.Where(i => i.WarehouseId == warehouse)
               .Select(i => new { ProductName = i.Name, Quantity = i.RemainingQuantity, cost = i.RemainingQuantity * i.PurchasePrice, Amount = i.RemainingQuantity * i.SalePrice }).ToArray();
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
            var catTre = db.LedgerAccounts.Where(i => i.ParentId == null).ToArray();

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

        public ContentResult GetPageControls(int id, string dfrom, string dto, string stime, string etime, int hit)
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
                tran = db.Transactions.Where(i => i.WarehouseId == warehouse).ToList();
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
        public ActionResult NProfit()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(i => i.Id == warehouses), "Id", "Name");
            ViewBag.UserId = new SelectList(db.Users.Where(n => n.RoleId == 2 || n.RoleId == 1 && n.WarehouseId == warehouses), "Id", "UserName");
            NProfitDto d = NProfitCombine(Datefrom, Dateto);

            return View(d);
        }


        [HttpPost]
        public ActionResult NProfit(string FromDate, string ToDate, string stime, string etime, int WarehouseId)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(i => i.Id == warehouses), "Id", "Name");
            ViewBag.UserId = new SelectList(db.Users.Where(n => n.RoleId == 2 || n.RoleId == 1 && n.WarehouseId == warehouses), "Id", "UserName");
            NProfitDto d = NProfitCombine(Datefrom, Dateto, WarehouseId);

            return View(d);
        }

        private NProfitDto NProfitCombine(DateTime Datefrom, DateTime Dateto, int? warehouse = null)
        {
            //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));


            System.Diagnostics.Debug.WriteLine("hapana : " + warehouse);
            //System.Diagnostics.Debug.WriteLine("hapana : " + user);
            if (warehouse != null)
            {
                var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
                var expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
                var sales = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
                var purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();

                NProfitDto d = new NProfitDto();

                d.Sale = sales.Sum(i => i.TotalAmount);
                d.Purchase = purchase.Sum(i => i.TotalAmount);

                d.Expense = expense.Sum(i => i.Amount);
                d.VCost = d.Sale - d.Purchase;
                d.Balance = d.VCost - d.Expense;
                d.companayname = invoiceFormat.CompanyName;
                return d;
            }
            else
            {

                //var invoiceFormat = db.InvoiceFormats.FirstOrDefault();
                //var sales = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                //var expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                //var purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouses);
                var expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouses).ToArray();
                var sales = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouses).ToArray();
                var purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouses).ToArray();
                NProfitDto d = new NProfitDto();

                d.Sale = sales.Sum(i => i.TotalAmount);
                d.Purchase = purchase.Sum(i => i.TotalAmount);

                d.Expense = expense.Sum(i => i.Amount);
                d.VCost = d.Sale - d.Purchase;
                d.Balance = d.VCost - d.Expense;
                d.companayname = invoiceFormat.CompanyName;
                return d;
            }

        }

        [HttpGet]
        public ActionResult Profit()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name");

            List<ProfitDto> listStock = new List<ProfitDto>();

            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            var sAlert1 = db.ProductStock.Where(i => (i.InventoryTypeId == 2) && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
               .Select(i => new { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, ProfitAmount = i.Profit, AmountWithTax = i.ProfitWithTax, SalePrice = i.SalePrice, PurchasePrice = i.PurchasePrice }).ToArray();

            var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
            foreach (var item in sAlert2)
            {
                ProfitDto li = new ProfitDto();
                li.ProductName = item;
                li.Quantity = sAlert1.Where(i => i.ProductName == item).Sum(i => i.Quantity);
                li.PurchasePrice = sAlert1.Where(i => i.ProductName == item).Sum(i => i.PurchasePrice);
                li.SalePrice = sAlert1.Where(i => i.ProductName == item).Sum(i => i.SalePrice);
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
            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name", ProductId);
                products = db.ProductStock.Where(i => (i.InventoryTypeId == 2) && i.ProductId == ProductId && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
               .Select(i => new ProfitDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, PurchasePrice = i.PurchasePrice, ProfitAmount = i.Profit, ProfitAmountWithTax = i.ProfitWithTax }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products.Where(m => m.WarehouseId == warehouse), "Id", "Name");
                products = db.ProductStock.Where(i => (i.InventoryTypeId == 2) && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                              .Select(i => new ProfitDto { ProductName = i.Product_ProductId.Name, SalePrice = i.SalePrice, PurchasePrice = i.PurchasePrice, Quantity = i.Quantity, ProfitAmount = i.Profit, ProfitAmountWithTax = i.ProfitWithTax }).ToList();
            }

            var sAlert2 = products.Select(i => i.ProductName).Distinct();
            foreach (var item in sAlert2)
            {
                ProfitDto li = new ProfitDto();
                li.ProductName = item;
                li.Quantity = products.Where(i => i.ProductName == item).Sum(i => i.Quantity);
                li.ProfitAmount = products.Where(i => i.ProductName == item).Sum(i => i.ProfitAmount);
                li.PurchasePrice = products.Where(i => i.ProductName == item).Sum(i => i.PurchasePrice);
                li.SalePrice = products.Where(i => i.ProductName == item).Sum(i => i.SalePrice);
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
            ViewBag.ProductId = new SelectList(db.Products.Where(i => i.WarehouseId == warehouse), "Id", "Name");

            SaleDto[] sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 4 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded.Value, companayname = invoiceFormat.CompanyName }).ToArray();
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
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            //List<SaleDto> sale = new List<SaleDto>();

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name", ProductId);
                //  sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 4 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                //  .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, SalePrice = i.SalePrice, Amount = i.TotalAmount, WithTaxAmount = i.TotalAmountWithTax, Dated = i.DateAdded.Value }).ToList();
                SaleDto[] sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 4 && i.WarehouseId == warehouse)
                 .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.ReturnedQuantity, SalePrice = i.PurchasePrice, Amount = Math.Round(i.PurchasePrice * i.ReturnedQuantity, 2), WithTaxAmount = Math.Round(i.PurchasePrice * i.ReturnedQuantity, 2), Dated = i.DateAdded.Value, companayname = invoiceFormat.CompanyName }).ToArray();
                return View(sale);
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name");
                SaleDto[] sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 4 && i.WarehouseId == warehouse)
             .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.ReturnedQuantity, SalePrice = i.PurchasePrice, Amount = Math.Round(i.PurchasePrice * i.ReturnedQuantity, 2), WithTaxAmount = Math.Round(i.PurchasePrice * i.ReturnedQuantity, 2), Dated = i.DateAdded.Value, companayname = invoiceFormat.CompanyName }).ToArray();
                return View(sale);
            }



        }



        [HttpGet]
        public ActionResult StockAdjustment()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouses), "Id", "Name");
            ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");

            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            StockAdjustmentDto[] sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto && i.InventoryTypeId == 7))
                .Select(i => new StockAdjustmentDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, Description = i.Description, InventoryType = i.InventoryType_InventoryTypeId.Name, AddedBy = db.Users.FirstOrDefault(u => u.Id == i.AddedBy).UserName, WareHouse = db.Warehouses.FirstOrDefault(w => w.Id == i.WarehouseId).Name, DateAdded = i.DateAdded }).ToArray();
            ViewBag.company = invoiceFormat.CompanyName;
            return View(sale);
        }
        [HttpPost]
        public ActionResult StockAdjustment(string FromDate, string ToDate, string stime, string etime, int? ProductId = null, int? IsFormalId = null)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 1);
            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            List<StockAdjustmentDto> sale = new List<StockAdjustmentDto>();

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.ProductId == ProductId && i.InventoryTypeId == 7)
             .Select(i => new StockAdjustmentDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, Description = i.Description, InventoryType = i.InventoryType_InventoryTypeId.Name, AddedBy = db.Users.FirstOrDefault(u => u.Id == i.AddedBy).UserName, WareHouse = db.Warehouses.FirstOrDefault(w => w.Id == i.WarehouseId).Name, DateAdded = i.DateAdded }).ToList();
                ViewBag.company = invoiceFormat.CompanyName;
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                ViewBag.IsFormalId = new SelectList(db.InvoiceTypes, "Id", "Name");
                sale = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto && i.InventoryTypeId == 7))
             .Select(i => new StockAdjustmentDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, Description = i.Description, InventoryType = i.InventoryType_InventoryTypeId.Name, AddedBy = db.Users.FirstOrDefault(u => u.Id == i.AddedBy).UserName, WareHouse = db.Warehouses.FirstOrDefault(w => w.Id == i.WarehouseId).Name, DateAdded = i.DateAdded }).ToList();
                ViewBag.company = invoiceFormat.CompanyName;
            }



            return View(sale);


        }




        [HttpGet]
        public ActionResult PurchaseReturn()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name");

            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            SaleDto[] Purchase = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 3 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, ReturnedQuantity = i.ReturnedQuantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalPurchaseAmount + i.TaxAmount, Dated = i.DateAdded.Value, companayname = invoiceFormat.CompanyName }).ToArray();
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
                ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name", ProductId);
                Purchase = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 3 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, ReturnedQuantity = i.ReturnedQuantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalPurchaseAmount + i.TaxAmount, Dated = i.DateAdded.Value }).ToList();
            }
            else
            {
                ViewBag.ProductId = new SelectList(db.Products.Where(n => n.WarehouseId == warehouse), "Id", "Name");
                Purchase = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 3 && i.WarehouseId == warehouse)
                .Select(i => new SaleDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, ReturnedQuantity = i.ReturnedQuantity, SalePrice = i.PurchasePrice, Amount = i.TotalPurchaseAmount, WithTaxAmount = i.TotalPurchaseAmount + i.TaxAmount, Dated = i.DateAdded.Value }).ToList();
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
                   .Select(i => new ExpenseDto { ExpenseName = (db.LedgerAccounts.FirstOrDefault(td => td.Id == i.ExpenseId).Name), expense = i.ExpenseId, VatNumber = i.VatNumber, Remarks = i.Remarks, InvoiceNumber = i.InvoiceNumber, VendorName = (db.Vendors.FirstOrDefault(td => td.Id == i.Vendorname).FullName), Vendorname = i.User_VendorUserId.FullName, subtotal = i.SubTotal, invoicedate = i.InvoiceDate, Amount = i.Amount, Dated = i.DateAdded, TaxAmount = i.TaxAmount, companayname = invoiceFormat.CompanyName }).ToArray();
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
                   .Select(i => new ExpenseDto { ExpenseName = (db.LedgerAccounts.FirstOrDefault(td => td.Id == i.ExpenseId).Name), expense = i.ExpenseId, VatNumber = i.VatNumber, Remarks = i.Remarks, InvoiceNumber = i.InvoiceNumber, VendorName = (db.Vendors.FirstOrDefault(td => td.Id == i.Vendorname).FullName), Vendorname = i.User_VendorUserId.FullName, subtotal = i.SubTotal, invoicedate = i.InvoiceDate, Amount = i.Amount, Dated = i.DateAdded, TaxAmount = i.TaxAmount, companayname = invoiceFormat.CompanyName }).ToArray();
            return View(expense);
        }

        [HttpGet]
        public ActionResult WarehouseStockAmount()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            // var products = db.Products.FirstOrDefault(i => i.WarehouseId == warehouse);
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");

            //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var myWarehouseName = db.Warehouses.FirstOrDefault(n => n.Id == warehouse).Name;
            if (warehouses == 1)
            {
                var products = db.Products;
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            }
            else
            {
                var products = db.Products.FirstOrDefault(i => i.WarehouseId == warehouse);
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(n => n.Id == warehouse), "Id", "Name");
            }


            List<WarehouseStockAmountDto> listStock = new List<WarehouseStockAmountDto>();
            List<WarehouseStockAmountDto> sAlert1 = new List<WarehouseStockAmountDto>();
            if (db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name == "")
            {
                //var mydate = db.StockTakes.FirstOrDefault(m => m.DateAdded == Datefrom)
                sAlert1 = db.StockTakeDetail.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                               .Select(i => new WarehouseStockAmountDto
                               {
                                   productId = i.ProductId,
                                   productName = db.Products.FirstOrDefault(n => n.Id == i.ProductId).Name,
                                   actualQuantity
                               = i.actualquantity,
                                   counted = i.counted,
                                   variance = i.variance,
                                   actualValue = i.actualvalue,
                                   varianceValue = i.variancevalue,
                                   stocktakeId = i.StockTakeId
                               }).ToList();


                ViewBag.company = invoiceFormat.CompanyName;
                ViewBag.branch = db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name;
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(i => i.Id == warehouse), "Id", "Name");
                return View(listStock);
            }

            sAlert1 = db.StockTakeDetail.Where(i => i.WarehouseId == warehouse && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
               .Select(i => new WarehouseStockAmountDto
               {
                   productId = i.ProductId,
                   productName = db.Products.FirstOrDefault(n => n.Id == i.ProductId).Name,
                   actualQuantity = i.actualquantity,
                   counted = i.counted,
                   variance = i.variance,
                   actualValue = i.actualvalue,
                   varianceValue = i.variancevalue,
                   stocktakeId = i.StockTakeId
               }).ToList();

            ViewBag.company = invoiceFormat.CompanyName;
            ViewBag.branch = db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name;
            return View(sAlert1);

        }
        [HttpPost]
        public ActionResult WarehouseStockAmount(string FromDate, string ToDate, string stime, string etime, int WarehouseId)
        {
            //var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            //  var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, WarehouseId);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            List<WarehouseStockAmountDto> listStock = new List<WarehouseStockAmountDto>();
            List<WarehouseStockAmountDto> sAlert1 = new List<WarehouseStockAmountDto>();
            if (WarehouseId > 0)
            {
                warehouse = WarehouseId;
            }

            if (db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name == " Dispatch")
            {
                sAlert1 = db.StockTakeDetail.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                               .Select(i => new WarehouseStockAmountDto
                               {
                                   productId = i.ProductId,
                                   productName = db.Products.FirstOrDefault(n => n.Id == i.ProductId).Name,
                                   actualQuantity
                               = i.actualquantity,
                                   counted = i.counted,
                                   variance = i.variance,
                                   actualValue = i.actualvalue,
                                   varianceValue = i.variancevalue,
                                   stocktakeId = i.StockTakeId
                               }).ToList();


                ViewBag.company = invoiceFormat.CompanyName;
                ViewBag.branch = db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name;
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(i => i.Id == warehouse), "Id", "Name");
                return View(listStock);
            }
            else
            {

                sAlert1 = db.StockTakeDetail.Where(i => i.WarehouseId == warehouse && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                               .Select(i => new WarehouseStockAmountDto
                               {
                                   productId = i.ProductId,
                                   productName = db.Products.FirstOrDefault(n => n.Id == i.ProductId).Name,
                                   actualQuantity
                               = i.actualquantity,
                                   counted = i.counted,
                                   variance = i.variance,
                                   actualValue = i.actualvalue,
                                   varianceValue = i.variancevalue,
                                   stocktakeId = i.StockTakeId
                               }).ToList();
            }

            ViewBag.company = invoiceFormat.CompanyName;
            ViewBag.branch = db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name;
            ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(i => i.Id == warehouse), "Id", "Name");
            return View(sAlert1);
        }


        [HttpGet]
        public ActionResult WareHouseStock()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var myWarehouseName = db.Warehouses.FirstOrDefault(n => n.Id == warehouse).Name;
            if (warehouses == 1)
            {
                var products = db.Products;
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            }
            else
            {
                var products = db.Products.FirstOrDefault(i => i.WarehouseId == warehouse && i.IsActive == true);
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(n => n.Id == warehouse), "Id", "Name");
            }

            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            List<WarehouseStockDto> listStock = new List<WarehouseStockDto>();
            List<WarehouseStockDto> sAlert1 = new List<WarehouseStockDto>();
            if (db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name == "")
            {
                sAlert1 = db.WarehouseStocks.Where(m => m.Product_ProductId.IsActive == true)
                               .Select(i => new WarehouseStockDto { ProductName = i.Product_ProductId.Name, Quantity = i.RemainingQuantity, ProductDescription = i.Product_ProductId.ProductDescription, StockValue = (i.Product_ProductId.SalePrice * i.RemainingQuantity), Price = i.Product_ProductId.SalePrice }).ToList();

                Product dst = new Product();
                var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
                foreach (var item in sAlert2)
                {
                    WarehouseStockDto li = new WarehouseStockDto();
                    var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);

                    li.ProductName = item;
                    li.Quantity = sAlert1.Where(i => i.ProductName == item).Sum(i => i.Quantity);
                    li.ProductDescription = db.Products.FirstOrDefault(i => i.Name == item).ProductDescription;

                    listStock.Add(li);


                }
                ViewBag.company = invoiceFormat.CompanyName;
                ViewBag.branch = db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name;
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(i => i.Id == warehouse), "Id", "Name");
                return View(listStock);
            }

            sAlert1 = db.WarehouseStocks.Where(i => i.WarehouseId == warehouse && i.Product_ProductId.IsActive == true)
               .Select(i => new WarehouseStockDto { ProductName = i.Product_ProductId.Name, Quantity = i.RemainingQuantity, ProductDescription = i.Product_ProductId.ProductDescription, StockValue = (i.Product_ProductId.SalePrice * i.RemainingQuantity), Price = i.Product_ProductId.SalePrice }).ToList();

            ViewBag.company = invoiceFormat.CompanyName;
            ViewBag.branch = db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name;
            return View(sAlert1);

        }


        [HttpPost]
        public ActionResult WareHouseStock(string FromDate, string ToDate, string stime, string etime, int WarehouseId)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var myWarehouseName = db.Warehouses.FirstOrDefault(n => n.Id == WarehouseId).Name;

            var products = db.Products.FirstOrDefault(i => i.WarehouseId == warehouse && i.IsActive == true);

            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            List<WarehouseStockDto> listStock = new List<WarehouseStockDto>();
            List<WarehouseStockDto> sAlert1 = new List<WarehouseStockDto>();
            if (WarehouseId > 0)
            {
                warehouse = WarehouseId;
            }

            if (db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name == "")
            {
                sAlert1 = db.WarehouseStocks.Where(n => n.Product_ProductId.IsActive == true)
                               .Select(i => new WarehouseStockDto { ProductName = i.Product_ProductId.Name, Quantity = i.RemainingQuantity, ProductDescription = i.Product_ProductId.ProductDescription, StockValue = (i.Product_ProductId.SalePrice * i.RemainingQuantity), Price = i.Product_ProductId.SalePrice }).ToList();

                Product dst = new Product();
                var warehousename = db.Warehouses.FirstOrDefault(m => m.Id == warehouse).Name;

                var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();


                foreach (var item in sAlert2)
                {
                    WarehouseStockDto li = new WarehouseStockDto();
                    var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);

                    li.ProductName = item;
                    li.Quantity = sAlert1.Where(i => i.ProductName == item).Sum(i => i.Quantity);
                    li.ProductDescription = db.Products.FirstOrDefault(i => i.Name == item).ProductDescription;
                    listStock.Add(li);


                }
                ViewBag.company = invoiceFormat.CompanyName;
                ViewBag.branch = db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name;
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                return View(listStock);
            }
            //else if (myWarehouseName == "Events")
            //{

            //    sAlert1 = db.WarehouseStocks.Where(i => i.WarehouseId == warehouse && i.Product_ProductId.productType =="CASE"|| i.Product_ProductId.productType =="SINGLE")
            //       .Select(i => new WarehouseStockDto { ProductName = i.Product_ProductId.Name, Quantity = i.RemainingQuantity, ProductDescription = i.Product_ProductId.ProductDescription }).ToList();
            //}
            else
            {
                sAlert1 = db.WarehouseStocks.Where(i => i.WarehouseId == warehouse && i.Product_ProductId.IsActive == true)
               .Select(i => new WarehouseStockDto { ProductName = i.Product_ProductId.Name, Quantity = i.RemainingQuantity, ProductDescription = i.Product_ProductId.ProductDescription, StockValue = (i.Product_ProductId.SalePrice * i.RemainingQuantity), Price = i.Product_ProductId.SalePrice }).ToList();
            }

            ViewBag.company = invoiceFormat.CompanyName;
            ViewBag.branch = db.Warehouses.FirstOrDefault(i => i.Id == warehouse).Name;
            ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(i => i.Id == warehouse), "Id", "Name");
            return View(sAlert1);
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
                .Select(i => new DueDto { Remarks = i.Remarks, Amount = i.DueAmount, Dated = i.DateAdded, IsReturn = i.IsReturn, companayname = invoiceFormat.CompanyName }).ToArray();

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
            if (IsReturn == null)
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
                .Select(i => new LadgerDto { Remarks = i.Remarks, Amount = i.CreditAmount, Dated = i.DateAdded, From = i.LedgerAccount_DebitLedgerAccountId.Name, To = i.LedgerAccount_CreditLedgerAccountId.Name, companayname = invoiceFormat.CompanyName }).ToArray();

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
            //var WarehouseId = Env.GetUserInfo()
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));


            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            if (warehouses == 1)
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            }
            else
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(n => n.Id == warehouses), "Id", "Name");
            }

            DayEndDto d = DayEndCombine(Datefrom, Dateto, warehouse);

            return View(d);
        }
        [HttpGet]
        public ActionResult DayEnd1()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            if (warehouses != 1)
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(m => m.Id == warehouse), "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(n => n.RoleId == 2 && n.IsActive == true && n.WarehouseId == warehouse), "Id", "UserName");
            }
            else
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(n => n.RoleId == 2 && n.IsActive == true), "Id", "UserName");
            }

            DayEndDto d = DayEndCombine1(Datefrom, Dateto);

            return View(d);
        }
        [HttpGet]
        public ActionResult DayEnd2()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            if (warehouses != 1)
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(m => m.Id == warehouse), "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(n => n.RoleId == 2 && n.IsActive == true && n.WarehouseId == warehouse), "Id", "UserName");
            }
            else
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(n => n.RoleId == 2 && n.IsActive == true), "Id", "UserName");
            }

            DayEndDto d = DayEndCombine2(Datefrom, Dateto);

            return View(d);
        }
        [HttpPost]
        public ActionResult DayEnd1(string FromDate, string ToDate, string stime, string etime, int UserId)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            if (warehouses != 1)
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(m => m.Id == warehouses), "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(n => n.RoleId == 2 && n.IsActive == true && n.WarehouseId == warehouses), "Id", "UserName");
            }
            else
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(n => n.RoleId == 2 && n.IsActive == true), "Id", "UserName");
            }
            DayEndDto d = DayEndCombine1(Datefrom, Dateto, UserId);

            return View(d);
        }

        [HttpPost]
        public ActionResult DayEnd2(string FromDate, string ToDate, string stime, string etime, int UserId)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            if (warehouses != 1)
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(m => m.Id == warehouses), "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(n => n.RoleId == 2 && n.IsActive == true && n.WarehouseId == warehouses), "Id", "UserName");
            }
            else
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
                ViewBag.UserId = new SelectList(db.Users.Where(n => n.RoleId == 2 && n.IsActive == true), "Id", "UserName");
            }
            DayEndDto d = DayEndCombine2(Datefrom, Dateto, UserId);

            return View(d);
        }

        private DayEndDto DayEndCombine1(DateTime Datefrom, DateTime Dateto, int? user = null)
        {
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            //  var warehouse = db.Users.FirstOrDefault(i => i.Id == user).WarehouseId;

            System.Diagnostics.Debug.WriteLine("hapana : " + user);
            System.Diagnostics.Debug.WriteLine("hapana : " + user);
            if (user != null)
            {
                var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
                var expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
                var accoutp = db.AccountPayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
                var due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();


                var ps = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.AddedBy == user)
                    .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.Profit, i.ProfitWithTax, i.InventoryTypeId }).ToArray();
                var paytrek = db.Paymenttracks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.AddedBy == user).ToArray();
                var productioncost = db.RawMaterialStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.AddedBy == user).ToArray();
                var finishedgoodsvalue = db.FinishedGoods.Where(i => (i.finisheddate >= Datefrom && i.finisheddate <= Dateto) && i.AddedBy == user).ToArray();
                var accountsale = db.Sales.Where(i => i.CustomerUserId != 3 && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.ModifiedBy == user).ToArray();

                var tsales = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.ModifiedBy == user)
                 .Select(i => new { i.InventoryTypeId, i.TotalAmountWithTax }).ToArray();
                var declared = db.DayEnds.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.AddedBy == user);
                var paymentm = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();


                expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                accoutp = db.AccountPayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();




                DayEndDto d = new DayEndDto();
                d.Sale = (decimal)tsales.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalAmountWithTax);
                //d.Sale = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalSaleAmountWithTax);
                d.PurchaseReturn = ps.Where(i => i.InventoryTypeId == 3).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
                d.DueReturn = due.Where(i => i.IsReturn == true).Sum(i => i.DueAmount);
                d.TotalPlus = (d.Sale + d.PurchaseReturn + d.DueReturn);

                d.Expense = expense.Sum(i => i.Amount);
                //d.productioncost = productioncost.Sum(i => i.TotalPurchaseAmount);
                //d.finishedgoodsvalue = finishedgoodsvalue.Sum(i => i.Total);
                d.DueGiven = due.Where(i => i.IsReturn == false).Sum(i => i.DueAmount);
                d.SaleReturn = ps.Where(i => i.InventoryTypeId == 4).Sum(i => i.TotalSaleAmountWithTax);
                d.Purchase = ps.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
                d.stock = ps.Where(i => i.InventoryTypeId == 5).Sum(i => i.TotalPurchaseAmount);
                d.stockplus = ps.Where(i => i.InventoryTypeId == 6).Sum(i => i.TotalPurchaseAmount);
                d.TotalMinus = (d.Expense + d.DueGiven + d.SaleReturn + d.Purchase);

                d.Profit = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.Profit);
                d.ProfitWithTax = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.ProfitWithTax);
                //payment track
                // d.rtgs = paytrek.Sum(i => i.swipe) + accoutp.Sum(i => i.swipe);
                // d.ecocash = paytrek.Sum(i => i.ecocash) + accoutp.Sum(i => i.ecocash);
                //  d.cashusd = paytrek.Sum(i => i.usd);
                d.rtgs = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ZIPIT").FirstOrDefault().Id && i.AddedBy == user && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                // d.rtgs = paytrek.Sum(i => i.swipe) + accoutp.Sum(i => i.swipe);
                d.ecocash = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ECOCASH").FirstOrDefault().Id && i.AddedBy == user && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.cashusd = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "USD").FirstOrDefault().Id && i.AddedBy == user && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.TotalAmount).DefaultIfEmpty(0).Sum();
                d.paymentmode = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "CASH").FirstOrDefault().Id && i.AddedBy == user && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.telecash = paytrek.Sum(i => i.telecash);
                d.onemoney = paytrek.Sum(i => i.onemoney);
                // d.paymentmode = paytrek.Sum(i => i.cash) + accoutp.Sum(i => i.cash);
                d.tSale = d.rtgs + d.ecocash + d.paymentmode;
                d.accountpayments = accoutp.Sum(i => i.Amount);
                d.accountsales = accountsale.Sum(i => i.TotalAmountWithTax);
                d.Change = paytrek.Sum(i => i.Change); /*+accoutp.Sum(i => i.Change)*/
                //payment track


                //declared
                d.cashdeclared = declared.Sum(i => i.totalcash);
                d.ecocashdeclared = declared.Sum(i => i.ecocash);
                d.telecashdeclared = declared.Sum(i => i.telecash);
                d.onemoneydeclared = declared.Sum(i => i.onemoney);
                d.cashusddeclared = declared.Sum(i => i.totalCashUsd);
                d.swipedeclared = declared.Sum(i => i.rtgs);
                d.totaldeclared = declared.Sum(i => i.totalAmount);
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
            else
            {

                var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
                var expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var accoutp = db.AccountPayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();


                var ps = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                    .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.Profit, i.ProfitWithTax, i.InventoryTypeId }).ToArray();
                var paytrek = db.Paymenttracks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var productioncost = db.RawMaterialStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var finishedgoodsvalue = db.FinishedItems.Where(i => (i.dateadded >= Datefrom && i.dateadded <= Dateto)).ToArray();
                var accountsale = db.Sales.Where(i => i.CustomerUserId != 3 && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();



                var tsales = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
              .Select(i => new { i.InventoryTypeId, i.TotalAmountWithTax }).ToArray();
                var declared = db.DayEnds.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto));
                var paymentm = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();

                expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                accoutp = db.AccountPayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();

                //var tsales = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                //                .Select(i => new { i.InventoryTypeId, i.TotalAmountWithTax }).ToArray();


                //ps = db.ProductStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                //   .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.Profit, i.ProfitWithTax, i.InventoryTypeId }).ToArray();
                //paymentm = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                //paytrek = db.Paymenttracks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                //productioncost = db.RawMaterialStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                //finishedgoodsvalue = db.FinishedItems.Where(i => (i.dateadded >= Datefrom && i.dateadded <= Dateto)).ToArray();
                //declared = db.DayEnds.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto));
                //accountsale = db.Sales.Where(i => i.CustomerUserId != 3 && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();


                DayEndDto d = new DayEndDto();
                d.Sale = (decimal)tsales.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalAmountWithTax);
                //d.Sale = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalSaleAmountWithTax);
                d.PurchaseReturn = ps.Where(i => i.InventoryTypeId == 3).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
                d.DueReturn = due.Where(i => i.IsReturn == true).Sum(i => i.DueAmount);
                d.TotalPlus = (d.Sale + d.PurchaseReturn + d.DueReturn);

                d.Expense = expense.Sum(i => i.Amount);
                //d.productioncost = productioncost.Sum(i => i.TotalPurchaseAmount);
                //d.finishedgoodsvalue = finishedgoodsvalue.Sum(i => i.Total);
                d.DueGiven = due.Where(i => i.IsReturn == false).Sum(i => i.DueAmount);
                d.SaleReturn = ps.Where(i => i.InventoryTypeId == 4).Sum(i => i.TotalSaleAmountWithTax);
                d.Purchase = ps.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
                d.stock = ps.Where(i => i.InventoryTypeId == 5).Sum(i => i.TotalPurchaseAmount);
                d.stockplus = ps.Where(i => i.InventoryTypeId == 6).Sum(i => i.TotalPurchaseAmount);
                d.TotalMinus = (d.Expense + d.DueGiven + d.SaleReturn + d.Purchase);

                d.Profit = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.Profit);
                d.ProfitWithTax = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.ProfitWithTax);
                //payment track
                d.rtgs = paytrek.Sum(i => i.swipe) + accoutp.Sum(i => i.swipe);
                d.ecocash = paytrek.Sum(i => i.ecocash) + accoutp.Sum(i => i.ecocash);
                //  d.cashusd = paytrek.Sum(i => i.usd);
                d.telecash = paytrek.Sum(i => i.telecash);
                d.onemoney = paytrek.Sum(i => i.onemoney);
                //   d.paymentmode = paytrek.Sum(i => i.cash) + accoutp.Sum(i => i.cash);
                d.tSale = d.rtgs + d.ecocash + d.paymentmode;
                d.accountpayments = accoutp.Sum(i => i.Amount);
                d.accountsales = accountsale.Sum(i => i.TotalAmountWithTax);
                d.Change = paytrek.Sum(i => i.Change); /*+accoutp.Sum(i => i.Change)*/
                d.rtgs = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ZIPIT").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                // d.rtgs = paytrek.Sum(i => i.swipe) + accoutp.Sum(i => i.swipe);
                d.ecocash = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ECOCASH").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.cashusd = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "USD").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.TotalAmount).DefaultIfEmpty(0).Sum();
                d.paymentmode = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "CASH").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();

                //payment track


                //declared
                d.cashdeclared = declared.Sum(i => i.totalcash);
                d.ecocashdeclared = declared.Sum(i => i.ecocash);
                d.telecashdeclared = declared.Sum(i => i.telecash);
                d.onemoneydeclared = declared.Sum(i => i.onemoney);
                d.cashusddeclared = declared.Sum(i => i.totalCashUsd);
                d.swipedeclared = declared.Sum(i => i.rtgs);
                d.totaldeclared = declared.Sum(i => i.totalAmount);
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


                d.companayname = invoiceFormat.CompanyName;
                return d;
            }

        }


        private DayEndDto DayEndCombine2(DateTime Datefrom, DateTime Dateto, int? user = null)
        {
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            //  var warehouse = db.Users.FirstOrDefault(i => i.Id == user).WarehouseId;

            System.Diagnostics.Debug.WriteLine("hapana : " + user);
            System.Diagnostics.Debug.WriteLine("hapana : " + user);
            if (user != null)
            {
                var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
                var expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
                var accoutp = db.AccountPayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
                var due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();


                var ps = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.AddedBy == user)
                    .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.Profit, i.ProfitWithTax, i.InventoryTypeId }).ToArray();
                var paytrek = db.Paymenttracks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.AddedBy == user).ToArray();
                var productioncost = db.RawMaterialStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.AddedBy == user).ToArray();
                var finishedgoodsvalue = db.FinishedGoods.Where(i => (i.finisheddate >= Datefrom && i.finisheddate <= Dateto) && i.AddedBy == user).ToArray();
                var accountsale = db.Sales.Where(i => i.CustomerUserId != 3 && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.ModifiedBy == user).ToArray();

                var tsales = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.ModifiedBy == user)
                 .Select(i => new { i.InventoryTypeId, i.TotalAmountWithTax }).ToArray();
                var declared = db.DayEnds.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.AddedBy == user);
                var paymentm = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();


                expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                accoutp = db.AccountPayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();




                DayEndDto d = new DayEndDto();
                d.Sale = (decimal)tsales.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalAmountWithTax);
                //d.Sale = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalSaleAmountWithTax);
                d.PurchaseReturn = ps.Where(i => i.InventoryTypeId == 3).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
                d.DueReturn = due.Where(i => i.IsReturn == true).Sum(i => i.DueAmount);
                d.TotalPlus = (d.Sale + d.PurchaseReturn + d.DueReturn);

                d.Expense = expense.Sum(i => i.Amount);
                //d.productioncost = productioncost.Sum(i => i.TotalPurchaseAmount);
                //d.finishedgoodsvalue = finishedgoodsvalue.Sum(i => i.Total);
                d.DueGiven = due.Where(i => i.IsReturn == false).Sum(i => i.DueAmount);
                d.SaleReturn = ps.Where(i => i.InventoryTypeId == 4).Sum(i => i.TotalSaleAmountWithTax);
                d.Purchase = ps.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
                d.stock = ps.Where(i => i.InventoryTypeId == 5).Sum(i => i.TotalPurchaseAmount);
                d.stockplus = ps.Where(i => i.InventoryTypeId == 6).Sum(i => i.TotalPurchaseAmount);
                d.TotalMinus = (d.Expense + d.DueGiven + d.SaleReturn + d.Purchase);

                d.Profit = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.Profit);
                d.ProfitWithTax = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.ProfitWithTax);
                //payment track
                // d.rtgs = paytrek.Sum(i => i.swipe) + accoutp.Sum(i => i.swipe);
                // d.ecocash = paytrek.Sum(i => i.ecocash) + accoutp.Sum(i => i.ecocash);
                //  d.cashusd = paytrek.Sum(i => i.usd);
                d.rtgs = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ZIPIT").FirstOrDefault().Id && i.AddedBy == user && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                // d.rtgs = paytrek.Sum(i => i.swipe) + accoutp.Sum(i => i.swipe);
                d.ecocash = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ECOCASH").FirstOrDefault().Id && i.AddedBy == user && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.cashusd = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "USD").FirstOrDefault().Id && i.AddedBy == user && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.TotalAmount).DefaultIfEmpty(0).Sum();
                d.paymentmode = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "CASH").FirstOrDefault().Id && i.AddedBy == user && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.telecash = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "CUSTOMERUSD").FirstOrDefault().Id && i.AddedBy == user && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.onemoney = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "CUSTOMERZWL").FirstOrDefault().Id && i.AddedBy == user && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                // d.paymentmode = paytrek.Sum(i => i.cash) + accoutp.Sum(i => i.cash);
                d.tSale = d.rtgs + d.ecocash + d.paymentmode;
                d.accountpayments = accoutp.Sum(i => i.Amount);
                d.accountsales = accountsale.Sum(i => i.TotalAmountWithTax);
                d.Change = paytrek.Sum(i => i.Change); /*+accoutp.Sum(i => i.Change)*/
                //payment track


                //declared
                d.cashdeclared = declared.Sum(i => i.totalcash);
                d.ecocashdeclared = declared.Sum(i => i.ecocash);
                d.telecashdeclared = declared.Sum(i => i.telecash);
                d.onemoneydeclared = declared.Sum(i => i.onemoney);
                d.cashusddeclared = declared.Sum(i => i.totalCashUsd);
                d.swipedeclared = declared.Sum(i => i.rtgs);
                d.totaldeclared = declared.Sum(i => i.totalAmount);
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
            else
            {

                var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
                var expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var accoutp = db.AccountPayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();


                var ps = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                    .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.Profit, i.ProfitWithTax, i.InventoryTypeId }).ToArray();
                var paytrek = db.Paymenttracks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var productioncost = db.RawMaterialStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var finishedgoodsvalue = db.FinishedItems.Where(i => (i.dateadded >= Datefrom && i.dateadded <= Dateto)).ToArray();
                var accountsale = db.Sales.Where(i => i.CustomerUserId != 3 && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();



                var tsales = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
              .Select(i => new { i.InventoryTypeId, i.TotalAmountWithTax }).ToArray();
                var declared = db.DayEnds.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto));
                var paymentm = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();

                expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                accoutp = db.AccountPayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();

                //var tsales = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                //                .Select(i => new { i.InventoryTypeId, i.TotalAmountWithTax }).ToArray();


                //ps = db.ProductStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                //   .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.Profit, i.ProfitWithTax, i.InventoryTypeId }).ToArray();
                //paymentm = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                //paytrek = db.Paymenttracks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                //productioncost = db.RawMaterialStocks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                //finishedgoodsvalue = db.FinishedItems.Where(i => (i.dateadded >= Datefrom && i.dateadded <= Dateto)).ToArray();
                //declared = db.DayEnds.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto));
                //accountsale = db.Sales.Where(i => i.CustomerUserId != 3 && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();


                DayEndDto d = new DayEndDto();
                d.Sale = (decimal)tsales.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalAmountWithTax);
                //d.Sale = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalSaleAmountWithTax);
                d.PurchaseReturn = ps.Where(i => i.InventoryTypeId == 3).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
                d.DueReturn = due.Where(i => i.IsReturn == true).Sum(i => i.DueAmount);
                d.TotalPlus = (d.Sale + d.PurchaseReturn + d.DueReturn);

                d.Expense = expense.Sum(i => i.Amount);
                //d.productioncost = productioncost.Sum(i => i.TotalPurchaseAmount);
                //d.finishedgoodsvalue = finishedgoodsvalue.Sum(i => i.Total);
                d.DueGiven = due.Where(i => i.IsReturn == false).Sum(i => i.DueAmount);
                d.SaleReturn = ps.Where(i => i.InventoryTypeId == 4).Sum(i => i.TotalSaleAmountWithTax);
                d.Purchase = ps.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
                d.stock = ps.Where(i => i.InventoryTypeId == 5).Sum(i => i.TotalPurchaseAmount);
                d.stockplus = ps.Where(i => i.InventoryTypeId == 6).Sum(i => i.TotalPurchaseAmount);
                d.TotalMinus = (d.Expense + d.DueGiven + d.SaleReturn + d.Purchase);

                d.Profit = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.Profit);
                d.ProfitWithTax = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.ProfitWithTax);
                //payment track
                d.rtgs = paytrek.Sum(i => i.swipe) + accoutp.Sum(i => i.swipe);
                d.ecocash = paytrek.Sum(i => i.ecocash) + accoutp.Sum(i => i.ecocash);
                //  d.cashusd = paytrek.Sum(i => i.usd);
                //d.telecash = paytrek.Sum(i => i.telecash);
                //d.onemoney = paytrek.Sum(i => i.onemoney);
                //   d.paymentmode = paytrek.Sum(i => i.cash) + accoutp.Sum(i => i.cash);

                d.accountpayments = accoutp.Sum(i => i.Amount);
                d.accountsales = accountsale.Sum(i => i.TotalAmountWithTax);
                d.Change = paytrek.Sum(i => i.Change); /*+accoutp.Sum(i => i.Change)*/
                d.rtgs = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ZIPIT").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                // d.rtgs = paytrek.Sum(i => i.swipe) + accoutp.Sum(i => i.swipe);
                d.ecocash = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ECOCASH").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.cashusd = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "USD").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.TotalAmount).DefaultIfEmpty(0).Sum();
                d.paymentmode = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "CASH").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.telecash = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "CUSTOMERUSD").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.onemoney = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "CUSTOMERZWL").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();

                d.tSale = d.rtgs + d.ecocash + d.paymentmode;
                //payment track


                //declared
                d.cashdeclared = declared.Sum(i => i.totalcash);
                d.ecocashdeclared = declared.Sum(i => i.ecocash);
                d.telecashdeclared = declared.Sum(i => i.telecash);
                d.onemoneydeclared = declared.Sum(i => i.onemoney);
                d.cashusddeclared = declared.Sum(i => i.totalCashUsd);
                d.swipedeclared = declared.Sum(i => i.rtgs);
                d.totaldeclared = declared.Sum(i => i.totalAmount);
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


                d.companayname = invoiceFormat.CompanyName;
                return d;
            }

        }



        [HttpPost]
        public ActionResult DayEnd(string FromDate, string ToDate, string stime, string etime, int? WarehouseId = null)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);

            BaseOfReport(stime, etime, Datefrom, Dateto, 0);
            if (WarehouseId != 1)
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses.Where(m => m.Id == WarehouseId), "Id", "Name");
            }
            else
            {
                ViewBag.WarehouseId = new SelectList(db.Warehouses, "Id", "Name");
            }

            DayEndDto d = DayEndCombine(Datefrom, Dateto, WarehouseId);

            return View(d);
        }


        private DayEndDto DayEndCombine(DateTime Datefrom, DateTime Dateto, int? warehouse = null)
        {
            //int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            System.Diagnostics.Debug.WriteLine("hapana : " + warehouse);
            if (warehouse != null)
            {
                var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
                var expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
                var accoutp = db.AccountPayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
                var due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();

                var ps = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                    .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.Profit, i.ProfitWithTax, i.InventoryTypeId }).ToArray();
                var paymentm = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
                var paytrek = db.Paymenttracks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
                var declared = db.DayEnds.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse);
                var accountsale = db.Sales.Where(i => i.CustomerUserId != 3 && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();

                DayEndDto d = new DayEndDto();

                d.Sale = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalSaleAmountWithTax);
                d.PurchaseReturn = ps.Where(i => i.InventoryTypeId == 3).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
                d.DueReturn = due.Where(i => i.IsReturn == true).Sum(i => i.DueAmount);
                d.TotalPlus = (d.Sale + d.PurchaseReturn + d.DueReturn);

                d.Expense = expense.Sum(i => i.Amount);
                d.DueGiven = due.Where(i => i.IsReturn == false).Sum(i => i.DueAmount);
                d.SaleReturn = ps.Where(i => i.InventoryTypeId == 4).Sum(i => i.TotalSaleAmountWithTax);
                d.Purchase = ps.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
                d.stock = ps.Where(i => i.InventoryTypeId == 5).Sum(i => i.TotalPurchaseAmount);
                d.stockplus = ps.Where(i => i.InventoryTypeId == 6).Sum(i => i.TotalPurchaseAmount);
                d.TotalMinus = (d.Expense + d.DueGiven + d.SaleReturn + d.Purchase);

                d.Profit = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.Profit);
                d.ProfitWithTax = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.ProfitWithTax);
                //payment track
                //var dattt = from sd in db.Sales.ToList()
                //            join pm in db.PaymentModes on sd.PaymentModeId equals pm.Id
                //            select new {
                //              ecocash =  db.Sales.Where(i=> i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ËCOCASH").First().Id).Sum(s => s.TotalRtgsAmount)
                //            };

                d.Fbc = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "FBC").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.TotalRtgsAmount).DefaultIfEmpty(0).Sum();
                d.Acl = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ACL").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.TotalRtgsAmount).DefaultIfEmpty(0).Sum();
                d.Zipit = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ZIPIT").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.rtgs = paytrek.Sum(i => i.swipe) + accoutp.Sum(i => i.swipe);
                d.ecocash = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ECOCASH").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();

                //d.ecocash = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ECOCASH").FirstOr().Id && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select( st =>  st.TotalRtgsAmount).DefaultIfEmpty(0).Sum();
                d.cashusd = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "USD").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.TotalAmount).DefaultIfEmpty(0).Sum();
                d.telecash = paytrek.Sum(i => i.telecash);
                d.onemoney = paytrek.Sum(i => i.onemoney);
                d.paymentmode = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "CASH").FirstOrDefault().Id && i.WarehouseId == warehouse && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.tSale = d.rtgs + d.ecocash + d.paymentmode;
                d.accountpayments = accoutp.Sum(i => i.Amount);
                d.accountsales = accountsale.Sum(i => i.TotalAmountWithTax);
                d.Change = paytrek.Sum(i => i.Change); /*+accoutp.Sum(i => i.Change)*/
                //payment track


                //declared 
                d.Fbcdeclared = declared.Sum(i => i.Fbc);
                d.Acldeclared = declared.Sum(i => i.Acl);
                d.Zipitdeclared = declared.Sum(i => i.Zipit);
                d.cashdeclared = declared.Sum(i => i.totalcash);
                d.ecocashdeclared = declared.Sum(i => i.ecocash);
                d.telecashdeclared = declared.Sum(i => i.telecash);
                d.onemoneydeclared = declared.Sum(i => i.onemoney);
                d.cashusddeclared = declared.Sum(i => i.totalCashUsd);
                d.swipedeclared = declared.Sum(i => i.rtgs);

                d.totaldeclared = declared.Sum(i => i.totalAmount);
                d.accumulatedchange = declared.Sum(i => i.totalChange);
                //declared
                //outages
                d.outagefbc = d.Fbcdeclared - d.Fbc;
                d.outagezipit = d.Zipitdeclared - d.Zipit;
                d.outageacl = d.Acldeclared - d.Acl;
                d.outagecash = d.cashdeclared - d.paymentmode;
                d.outageecocash = d.ecocash - d.ecocashdeclared;
                d.outagetelecash = d.telecash - d.telecashdeclared;
                d.outageonemoney = d.onemoney - d.onemoneydeclared;
                d.outagecashusd = d.cashusddeclared - d.cashusd;
                d.outageswipe = d.rtgs - d.swipedeclared;
                d.outageChange = d.Change - d.cashdeclared;
                d.outagetotal = d.tSale - (d.cashdeclared + d.ecocashdeclared + d.swipedeclared);


                //outage


                d.companayname = invoiceFormat.CompanyName;
                return d;
            }
            else
            {
                var invoiceFormat = db.InvoiceFormats.FirstOrDefault();
                var expense = db.Expenses.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var accoutp = db.AccountPayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var due = db.DuePayments.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();

                var ps = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto))
                    .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.Profit, i.ProfitWithTax, i.InventoryTypeId }).ToArray();
                var paymentm = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var paytrek = db.Paymenttracks.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();
                var declared = db.DayEnds.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto));
                var accountsale = db.Sales.Where(i => i.CustomerUserId != 3 && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).ToArray();

                DayEndDto d = new DayEndDto();
                d.Sale = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalSaleAmountWithTax);
                d.PurchaseReturn = ps.Where(i => i.InventoryTypeId == 3).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
                d.DueReturn = due.Where(i => i.IsReturn == true).Sum(i => i.DueAmount);
                d.TotalPlus = (d.Sale + d.PurchaseReturn + d.DueReturn);

                d.Expense = expense.Sum(i => i.Amount);
                d.DueGiven = due.Where(i => i.IsReturn == false).Sum(i => i.DueAmount);
                d.SaleReturn = ps.Where(i => i.InventoryTypeId == 4).Sum(i => i.TotalSaleAmountWithTax);
                d.Purchase = ps.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalPurchaseAmount + i.TaxAmount);
                d.stock = ps.Where(i => i.InventoryTypeId == 5).Sum(i => i.TotalPurchaseAmount);
                d.stockplus = ps.Where(i => i.InventoryTypeId == 6).Sum(i => i.TotalPurchaseAmount);
                d.TotalMinus = (d.Expense + d.DueGiven + d.SaleReturn + d.Purchase);

                d.Profit = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.Profit);
                d.ProfitWithTax = ps.Where(i => i.InventoryTypeId == 2).Sum(i => i.ProfitWithTax);
                //payment track
                d.Fbc = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "FBC").FirstOrDefault().Id && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.Acl = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ACL").FirstOrDefault().Id && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.Zipit = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ZIPIT").FirstOrDefault().Id && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.rtgs = paytrek.Sum(i => i.swipe) + accoutp.Sum(i => i.swipe);
                d.ecocash = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "ECOCASH").FirstOrDefault().Id && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();

                // d.ecocash = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "FBC").FirstOrDefault().Id && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto)).Sum(s => (decimal) s.TotalRtgsAmount);
                d.cashusd = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "USD").FirstOrDefault().Id && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.TotalAmount).DefaultIfEmpty(0).Sum();
                d.telecash = paytrek.Sum(i => i.telecash);
                d.onemoney = paytrek.Sum(i => i.onemoney);
                d.paymentmode = db.Sales.Where(i => i.PaymentModeId == db.PaymentModes.Where(t => t.Name == "CASH").FirstOrDefault().Id && i.DateAdded >= Datefrom && i.DateAdded <= Dateto).Select(st => st.rtgs).DefaultIfEmpty(0).Sum();
                d.tSale = d.rtgs + d.ecocash + d.paymentmode;
                d.accountpayments = accoutp.Sum(i => i.Amount);
                d.accountsales = accountsale.Sum(i => i.TotalAmountWithTax);
                d.Change = paytrek.Sum(i => i.Change); /*+accoutp.Sum(i => i.Change)*/
                //payment track


                //declared
                d.Fbcdeclared = declared.Sum(i => i.Fbc);
                d.Acldeclared = declared.Sum(i => i.Acl);
                d.Zipitdeclared = declared.Sum(i => i.Zipit);
                d.cashdeclared = declared.Sum(i => i.totalcash);
                d.ecocashdeclared = declared.Sum(i => i.ecocash);
                d.telecashdeclared = declared.Sum(i => i.telecash);
                d.onemoneydeclared = declared.Sum(i => i.onemoney);
                d.cashusddeclared = declared.Sum(i => i.totalCashUsd);
                d.swipedeclared = declared.Sum(i => i.rtgs);
                d.totaldeclared = declared.Sum(i => i.totalAmount);
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
                d.outagefbc = d.Fbc - d.Fbcdeclared;
                d.outagezipit = d.Zipit - d.Zipitdeclared;
                d.outageacl = d.Acl - d.Acldeclared;

                //outage


                d.companayname = invoiceFormat.CompanyName;
                return d;
            }


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
                .Select(i => new PurchaseDto { ProductName = i.Product_ProductId.Name, supplierId = i.Vendor_Id.UserName, Quantity = i.Quantity, TotalPuchaseAmount = i.TotalAmount, Dated = i.DateAdded, companayname = invoiceFormat.CompanyName, AddedBy = db.Users.FirstOrDefault(u => u.Id == i.AddedBy).UserName, InvoiceNumber = i.InvoiceNumber }).ToArray();
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
               .Select(i => new { i.Product_ProductId.Name, i.Quantity, i.TotalAmount, i.DateAdded, i.VendorUserId, i.AddedBy }).ToArray();

            var ps = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.InventoryTypeId }).ToArray();

            var vendor = db.Users.Where(i => i.RoleId == 3)
                .Select(i => new { i.UserName, i.Id }).ToArray();

            List<PurchaseDto> Purchase = new List<PurchaseDto>();

            if (ProductId != null)
            {
                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", ProductId);
                // foreach (var ven in vendor)
                //  {



                Purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.ProductId == ProductId && i.WarehouseId == warehouse)
                .Select(i => new PurchaseDto { ProductName = i.Product_ProductId.Name, supplierId = i.Vendor_Id.UserName, Quantity = i.Quantity, TotalPuchaseAmount = i.TotalAmount, Dated = i.DateAdded, AddedBy = db.Users.FirstOrDefault(u => u.Id == i.AddedBy).UserName }).ToList()

                ;
            }
            //    }
            else
            {
                //  foreach (var ven in vendor)
                //  {

                ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
                Purchase = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.InventoryTypeId == 1 && i.WarehouseId == warehouse)
                .Select(i => new PurchaseDto { ProductName = i.Product_ProductId.Name, supplierId = i.Vendor_Id.UserName, Quantity = i.Quantity, TotalPuchaseAmount = i.TotalAmount, Dated = i.DateAdded, AddedBy = db.Users.FirstOrDefault(u => u.Id == i.AddedBy).UserName }).ToList();
                //  }
            }


            return View(Purchase);
        }
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
        public ActionResult VAT(string FromDate, string ToDate, string stime, string etime)
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(string.Format(FromDate, "d/M/yyyy")), stime);
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
                .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.InventoryTypeId }).ToArray();
            var sales = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse && i.isFiscalised == true)
                .Select(i => new { i.TotalAmount, i.TotalAmountWithTax, i.InventoryTypeId, i.PaymentModeId }).ToArray();
            var sales1 = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse && i.isFiscalised == true)
              .Select(i => new { i.TotalAmount, i.TotalAmountWithTax, i.InventoryTypeId }).ToArray();
            var purchases = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
                .Select(i => new { i.TotalAmount, i.TotalAmountWithTax, i.InventoryTypeId, i.TaxAmount }).ToArray();
            var Credits = db.DebitCreditNotes.Where(i => (i.Duedate >= Datefrom && i.Duedate <= Dateto) && i.WarehouseId == warehouse)
                 .Select(i => new { i.total, i.vat, i.Status, i.ReceiptCurrency }).ToArray();

            VATReport d = new VATReport();
            d.Sale = sales.Where(i => i.InventoryTypeId == 2 && (i.PaymentModeId == 6 || i.PaymentModeId == 2016)).Sum(i => i.TotalAmount + (decimal)i.TotalAmountWithTax);
            d.salesexcludevat = sales.Where(i => i.InventoryTypeId == 2 && i.PaymentModeId == 6 || i.PaymentModeId == 2016).Sum(i => i.TotalAmount);
            // d.taxablesales = ps.Where(i => /*i.CGST == 2 &&*/ i.InventoryTypeId == 2).Sum(i => i.TotalSaleAmountWithTax - i.TaxAmount);
            //  d.nontaxablesales = ps.Where(i => i.InventoryTypeId == 2 /*& i.CGST == 5*/).Sum(i => i.TotalSaleAmountWithTax);
            d.SaleReturn = Credits.Where(i => i.ReceiptCurrency == "USD" && i.Status == "Success").Sum(i => i.vat);
            d.totalPurchase = purchases.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalAmountWithTax);
            d.taxablepurchases = purchases.Where(i => i.InventoryTypeId == 1 /*&& i.CGST==2*/).Sum(i => i.TotalAmount - i.TaxAmount);
            d.nontaxablepurchase = ps.Where(i => i.InventoryTypeId == 2/* && i.CGST == 5*/).Sum(i => i.TotalPurchaseAmount);
            d.Totapurchaseexcludingvat = purchases.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalAmount);
            d.taxonpurchases = purchases.Where(i => i.InventoryTypeId == 1).Sum(i => i.TaxAmount);
            d.taxonsales = (decimal)sales.Where(i => i.InventoryTypeId == 2 && i.PaymentModeId == 6 || i.PaymentModeId == 2016).Sum(i => i.TotalAmountWithTax) - (decimal)d.SaleReturn;
            d.nettax = d.taxonsales - d.taxonpurchases;
            d.datefrom = Datefrom;
            d.dateto = Dateto;
            d.companayname = invoiceFormat.CompanyName;

            // VATReport d = new VATReport();
            d.SaleZwg = sales.Where(i => i.InventoryTypeId == 2 && (i.PaymentModeId != 6 && i.PaymentModeId != 2016)).Sum(i => i.TotalAmount + (decimal)i.TotalAmountWithTax);
            d.salesexcludevatZwg = sales.Where(i => i.InventoryTypeId == 2 && (i.PaymentModeId != 6 && i.PaymentModeId != 2016)).Sum(i => i.TotalAmount);
            //  d.taxablesalesZwg = ps.Where(i => /*i.CGST == 2 &&*/ i.InventoryTypeId == 2).Sum(i => i.TotalSaleAmountWithTax - i.TaxAmount);
            //  d.nontaxablesales = ps.Where(i => i.InventoryTypeId == 2 /*& i.CGST == 5*/).Sum(i => i.TotalSaleAmountWithTax);
            d.SaleReturnZwg = Credits.Where(i => i.ReceiptCurrency == "ZWG" && i.Status == "Success").Sum(i => i.vat);
            d.totalPurchaseZwg = purchases.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalAmountWithTax);
            d.taxablepurchasesZwg = purchases.Where(i => i.InventoryTypeId == 1 /*&& i.CGST==2*/).Sum(i => i.TotalAmount - i.TaxAmount);
            d.nontaxablepurchaseZwg = ps.Where(i => i.InventoryTypeId == 2/* && i.CGST == 5*/).Sum(i => i.TotalPurchaseAmount);
            d.TotapurchaseexcludingvatZwg = purchases.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalAmount);
            d.taxonpurchasesZwg = purchases.Where(i => i.InventoryTypeId == 1).Sum(i => i.TaxAmount);
            d.taxablesalesZwg = (decimal)sales.Where(i => i.InventoryTypeId == 2 && (i.PaymentModeId != 6 && i.PaymentModeId != 2016)).Sum(i => i.TotalAmountWithTax) - (decimal)d.SaleReturnZwg;
            d.nettaxZwg = d.taxablesalesZwg - 0;
            d.datefrom = Datefrom;
            d.dateto = Dateto;
            d.companayname = invoiceFormat.CompanyName;


            return d;
        }


        //VAT report
        //[HttpGet]
        //public ActionResult VAT()
        //{
        //    var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
        //    var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
        //    //retrunDate = Convert.ToDateTime(finalDate.ToString("d/M/yyyy"));
        //    BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

        //    VATReport d = VAT(Datefrom, Dateto);

        //    return View(d);
        //}
        //[HttpPost]
        //public ActionResult VAT(string FromDate, string ToDate, string stime, string etime)
        //{
        //    var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(string.Format(FromDate, "d/M/yyyy")), stime);
        //    var Dateto = Env.AddTimeInDate(Convert.ToDateTime(string.Format(ToDate, "d/M/yyyy")), etime);

        //    BaseOfReport(stime, etime, Datefrom, Dateto, 0);

        //    VATReport d = VAT(Datefrom, Dateto);

        //    return View(d);
        //}
        //private VATReport VAT(DateTime Datefrom, DateTime Dateto)
        //{
        //    int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
        //    var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
        //    var purchasess = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();

        //    var ps = db.ProductStock.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse && i.IsFormal == true)
        //        .Select(i => new { i.TotalSaleAmountWithTax, i.TotalPurchaseAmount, i.TaxAmount, i.InventoryTypeId }).ToArray();
        //    var sales = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse && i.isFiscalised == true)
        //        .Select(i => new { i.TotalAmount, i.TotalAmountWithTax,  i.InventoryTypeId,i.Product_ProductId.TaxId }).ToArray();
        //    var purchases = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
        //        .Select(i => new { i.TotalAmount, i.TotalAmountWithTax, i.InventoryTypeId ,i.Product_ProductId.TaxId, i.TaxAmount}).ToArray();


        //    VATReport d = new VATReport();
        //    d.Sale = sales.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalAmount + (decimal)i.TotalAmountWithTax);
        //    d.salesexcludevat = sales.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalAmount );
        //    d.taxablesales = sales.Where(i => i.TaxId == 2 && i.InventoryTypeId == 2 ).Sum(i => i.TotalAmount);
        //    d.nontaxablesales = sales.Where(i => i.InventoryTypeId == 2 && i.TaxId != 2).Sum(i => i.TotalAmount);
        //    d.SaleReturn = ps.Where(i => i.InventoryTypeId == 4).Sum(i => i.TotalSaleAmountWithTax);
        //    d.totalPurchase = purchases.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalAmountWithTax);
        //    d.taxablepurchases = purchases.Where(i => i.InventoryTypeId == 1 && i.TaxId == 2).Sum(i => i.TotalAmount - i.TaxAmount);
        //    d.nontaxablepurchase = purchases.Where(i => i.InventoryTypeId == 2 && i.TaxId != 2).Sum(i => i.TotalAmount);
        //    d.Totapurchaseexcludingvat = purchases.Where(i => i.InventoryTypeId == 1).Sum(i => i.TotalAmount);
        //    d.taxonpurchases = purchases.Where(i => i.InventoryTypeId == 1).Sum(i => i.TaxAmount);
        //    d.taxonsales = (decimal)sales.Where(i => i.InventoryTypeId == 2).Sum(i => i.TotalAmountWithTax);
        //    d.nettax = d.taxonsales - d.taxonpurchases;
        //    d.datefrom = Datefrom;
        //    d.dateto = Dateto;
        //    d.companayname = invoiceFormat.CompanyName;

        //    return d;
        //}

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
            var paymentm = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse).ToArray();
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
            d.outagecasho = d.cashdeclared - d.cashold;
            d.outageecocasho = d.ecocashdeclared - d.ecocashold;
            d.outageswipeo = d.swipedeclared - d.swipeold;
            d.outagetotalo = d.outageswipeo + d.outageecocasho + d.outagecasho;
            //outage old
            return d;
        }


        private void BaseOfReport(string stime, string etime, DateTime Datefrom, DateTime Dateto, int IsGet)
        {
            ViewBag.FromDate = Datefrom.ToString("dd/MMM/yyyy");
            ViewBag.ToDate = Dateto.ToString("dd/MMM/yyyy");

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
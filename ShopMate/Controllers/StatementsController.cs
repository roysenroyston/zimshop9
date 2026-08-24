using ShopMate.ModelDto;
using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class StatementsController : Controller
    {
        private SIContext db = new SIContext();
        // GET: Statements
        public ActionResult Index()
        {
            ViewBag.CustomerId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Supplier" || i.Role_RoleId.RoleName == "Vendor"), "Id", "UserName");


            return View();
        }
        [HttpGet]
        public ActionResult CustomerStatement()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            ViewBag.CustomerId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Customer"), "Id", "UserName");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var products = db.Products.FirstOrDefault(i => i.WarehouseId == warehouse);
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            List<CustomerDto> listStock = new List<CustomerDto>();

            var sAlert1 = db.Sales.Where(i => (i.InventoryTypeId == 4 || i.InventoryTypeId == 2) && i.WarehouseId == warehouse)
               .Select(i => new { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, InvTypeId = i.InventoryTypeId, saleprice = i.SalePrice, Amount = i.TotalAmountWithTax }).ToArray();
            var ct = db.Products.OrderBy(i => i.ProductCategoryId);

            CustomerDto dst = new CustomerDto();
            var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
            foreach (var item in sAlert2)
            {
                CustomerDto li = new CustomerDto();
                var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);
                  li.ProductName = item;
                  li.Quantity = sAlert1.Where(i => (i.InvTypeId == 2 || i.InvTypeId==4) && i.ProductName == item).Sum(i => i.Quantity) ;
                // li.cost = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.cost) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.cost) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.cost) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.cost);
                // li.Amount = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Amount) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Amount) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Amount) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Amount);
                // li.cost = Math.Round(li.Quantity * selectedProduct.PurchasePrice, 2);
                // li.totalamount = Math.Round(li.Quantity * selectedProduct.SalePrice, 2);
                //li.purchaseprice = selectedProduct.PurchasePrice;
                //  li.saleprice = selectedProduct.SalePrice;
                listStock.Add(li);
            }
            ViewBag.company = invoiceFormat.CompanyName;
            ViewBag.Logo = Env.GetSiteRoot() + "/Uploads/" + invoiceFormat.Logo;
            return View(listStock);

        }



        [HttpPost]
        public ActionResult CustomerStatement(string FromDate, string ToDate, string stime, string etime, int CustomerId )
        {

            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
           
          
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            var userz = db.Users.FirstOrDefault(i => i.WarehouseId == warehouse && i.Id==CustomerId);
            var invoice = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            List<CustomerDto> sale = new List<CustomerDto>();
           
                ViewBag.CustomerName = userz.UserName;
                ViewBag.CustomerId = new SelectList(db.Users, "Id", "UserName");
                sale = db.Sales.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) &&( i.InventoryTypeId == 2 || i.InventoryTypeId == 4) && i.CustomerUserId == CustomerId && i.WarehouseId == warehouse)
                .Select(i => new CustomerDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, saleprice = i.SalePrice, totalamount = i.TotalAmountWithTax, dated = i.DateAdded.Value }).ToList();
                ViewBag.company = invoiceFormat.CompanyName;
                ViewBag.address = userz.Address;
                ViewBag.mobile = userz.Mobile;
                ViewBag.addressinfo = invoiceFormat.AddressInfo;
                ViewBag.Logo = Env.GetSiteRoot() + "/Uploads/" + invoiceFormat.Logo;
          
                return View(sale);


        }

        // supplier
        [HttpGet]
        public ActionResult Supplier()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");
            ViewBag.CustomerId = new SelectList(db.Users.Where(i => i.Role_RoleId.RoleName == "Supplier" || i.Role_RoleId.RoleName=="Vendor"), "Id", "UserName");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var products = db.Products.FirstOrDefault(i => i.WarehouseId == warehouse);
            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            List<SupplierDto> listStock = new List<SupplierDto>();

            var sAlert1 = db.Purchases.Where(i => (i.InventoryTypeId == 1 || i.InventoryTypeId == 3) && i.WarehouseId == warehouse)
               .Select(i => new { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, InvTypeId = i.InventoryTypeId, saleprice = i.UnitPrice, Amount = i.TotalAmountWithTax }).ToArray();
            var ct = db.Products.OrderBy(i => i.ProductCategoryId);

            SupplierDto dst = new SupplierDto();
            var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
            foreach (var item in sAlert2)
            {
                SupplierDto li = new SupplierDto();
                var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);
                li.ProductName = item;
                li.Quantity = sAlert1.Where(i => (i.InvTypeId == 1 || i.InvTypeId==3) && i.ProductName == item).Sum(i => i.Quantity);
                //li.cost = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.cost) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.cost) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.cost) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.cost);
                //li.Amount = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Amount) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Amount) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Amount) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Amount);
                //li.cost = Math.Round(li.Quantity * selectedProduct.PurchasePrice, 2);
                //li.totalamount = Math.Round(li.Quantity * selectedProduct.SalePrice, 2);
                //li.purchaseprice = selectedProduct.PurchasePrice;
                li.saleprice = selectedProduct.SalePrice;
                listStock.Add(li);
            }
            ViewBag.company = invoiceFormat.CompanyName;
            ViewBag.Logo = Env.GetSiteRoot() + "/Uploads/" + invoiceFormat.Logo;
            return View(listStock);

        }



        [HttpPost]
        public ActionResult Supplier(string FromDate, string ToDate, string stime, string etime, int CustomerId)
        {

            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(FromDate), stime);
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(ToDate), etime);
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));


            var invoiceFormat = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);
            var userz = db.Users.FirstOrDefault(i => i.WarehouseId == warehouse && i.Id == CustomerId);
            var invoice = db.InvoiceFormats.FirstOrDefault(i => i.WarehouseId == warehouse);

            List<SupplierDto> sale = new List<SupplierDto>();
           
            ViewBag.CustomerName = userz.UserName;
            ViewBag.CustomerId = new SelectList(db.Users, "Id", "UserName");
            sale = db.Purchases.Where(i => (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && (i.InventoryTypeId == 1 || i.InventoryTypeId == 3) && i.VendorUserId == CustomerId && i.WarehouseId == warehouse)
            .Select(i => new SupplierDto { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, saleprice = i.UnitPrice, totalamount = i.TotalAmountWithTax, dated = i.DateAdded }).ToList();
            ViewBag.company = invoiceFormat.CompanyName;
            ViewBag.address = userz.Address;
            ViewBag.mobile = userz.Mobile;
            ViewBag.addressinfo = invoiceFormat.AddressInfo;
            ViewBag.Logo = Env.GetSiteRoot() + "/Uploads/" + invoiceFormat.Logo;
            // }
            return View(sale);


        }

        // GET: Statements/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        
        // GET: Statements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Statements/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View();
            }
        }

        // GET: Statements/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Statements/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View();
            }
        }

        // GET: Statements/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Statements/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                Helper.WriteError(ex, ex.Message);
                return View();
            }
        }
        private void BaseOfReport(string stime, string etime, DateTime Datefrom, DateTime Dateto, int IsGet)
        {
            ViewBag.FromDate = Datefrom.ToString("dd/MM/yyyy");
            ViewBag.ToDate = Datefrom.ToString("dd/MM/yyyy");

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

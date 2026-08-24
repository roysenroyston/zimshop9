using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopMate.Models;
using ShopMate.ModelDto;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class AccountingController : Controller
    {
        SIContext db = new SIContext();
        // GET: Accounting
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult trialBalance()
         {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
        var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

        BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

        ProfitorlossDto d = ProfitandLossaccount(Datefrom, Dateto);

            return View(d);
    }
    //trading profit and loss account
    [HttpGet]
        public ActionResult ProfitnLoss()
        {
            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(DateTime.Now), "23:59");

            BaseOfReport("00:01", "23:59", Datefrom, Dateto, 1);

            ProfitorlossDto d = ProfitandLossaccount(Datefrom, Dateto);

            return View(d);
        }
        [HttpGet]
        public ProfitorlossDto ProfitandLossaccount(DateTime fromd, DateTime toDate)
        {

            var Datefrom = Env.AddTimeInDate(Convert.ToDateTime(fromd), "00:01");
            var Dateto = Env.AddTimeInDate(Convert.ToDateTime(toDate), "23:59");


            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");

            ProfitorlossDto p = new ProfitorlossDto();
            int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
            var possale = db.ProductStock.Where(i => (i.InventoryTypeId == 2) && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse);
            var sreturns = db.ProductStock.Where(i => (i.InventoryTypeId == 4) && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse);
            var preturn = db.ProductStock.Where(i => (i.InventoryTypeId == 3) && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse);
            var purchas = db.ProductStock.Where(i => (i.InventoryTypeId == 1) && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse);

            var invoicesale = db.Invoices.Where(i => (i.IsPurchaseOrSale == "Sale") && (i.DateAdded >= Datefrom && i.DateAdded <= Dateto) && i.WarehouseId == warehouse);
            var sAlert1 = db.ProductStock.Where(i => (i.InventoryTypeId == 1 || i.InventoryTypeId == 2 || i.InventoryTypeId == 5 || i.InventoryTypeId == 6) && ( i.DateAdded < Dateto) && i.WarehouseId == warehouse)
               .Select(i => new { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, InvTypeId = i.InventoryTypeId, cost = i.TotalPurchaseAmount, Amount = i.TotalSaleAmountWithTax }).ToArray();
            var sAlert2 = sAlert1.Select(i => i.ProductName).Distinct();
            List<StockAmountDto> listStock = new List<StockAmountDto>();
            var temptotalstock=0.00m;
          
            foreach (var item in sAlert2)
            {
                StockAmountDto li = new StockAmountDto();
                var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);
                li.ProductName = item;
                li.Quantity = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Quantity) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Quantity);
                // li.cost = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.cost) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.cost) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.cost) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.cost);
                // li.Amount = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Amount) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Amount) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Amount) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Amount);
                li.cost = Math.Round(li.Quantity * selectedProduct.PurchasePrice, 2);
                li.Amount = Math.Round(li.Quantity * selectedProduct.SalePrice, 2);
                li.purchaseprice = selectedProduct.PurchasePrice;
                li.saleprice = selectedProduct.SalePrice;
                temptotalstock += li.cost;
                listStock.Add(li);
            }
            var sAlert3 = db.ProductStock.Where(i => (i.InventoryTypeId == 1 || i.InventoryTypeId == 2 || i.InventoryTypeId == 5 || i.InventoryTypeId == 6) && (i.DateAdded <= Dateto) && i.WarehouseId == warehouse)
              .Select(i => new { ProductName = i.Product_ProductId.Name, Quantity = i.Quantity, InvTypeId = i.InventoryTypeId, cost = i.TotalPurchaseAmount, Amount = i.TotalSaleAmountWithTax }).ToArray();
            var sAlert4 = sAlert3.Select(i => i.ProductName).Distinct();
            List<StockAmountDto> listclossStock = new List<StockAmountDto>();
            var tempclossingstock = 0.00m;

            foreach (var item in sAlert4)
            {
                StockAmountDto li = new StockAmountDto();
                var selectedProduct = db.Products.FirstOrDefault(i => i.Name == item);
                li.ProductName = item;
                li.Quantity = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Quantity) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Quantity) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Quantity);
                // li.cost = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.cost) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.cost) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.cost) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.cost);
                // li.Amount = sAlert1.Where(i => i.InvTypeId == 1 && i.ProductName == item).Sum(i => i.Amount) + sAlert1.Where(i => i.InvTypeId == 6 && i.ProductName == item).Sum(i => i.Amount) - sAlert1.Where(i => i.InvTypeId == 2 && i.ProductName == item).Sum(i => i.Amount) - sAlert1.Where(i => i.InvTypeId == 5 && i.ProductName == item).Sum(i => i.Amount);
                li.cost = Math.Round(li.Quantity * selectedProduct.PurchasePrice, 2);
                li.Amount = Math.Round(li.Quantity * selectedProduct.SalePrice, 2);
                li.purchaseprice = selectedProduct.PurchasePrice;
                li.saleprice = selectedProduct.SalePrice;
                tempclossingstock += li.cost;
                listStock.Add(li);
            }

            if (possale.Any()){ 
            p.Saless =  possale.Sum(i => i.TotalSaleAmountWithTax) ;}
            if(sreturns.Any()){ 
            p.salesreturns = sreturns.Sum(i => i.TotalSaleAmountWithTax);}
            if(preturn.Any()){ 
            p.purchasesreturns = preturn.Sum(i => i.TotalPurchaseAmount);}
            if (purchas.Any()) { 
            p.purchases = purchas.Sum(i => i.TotalPurchaseAmount);}
            p.openingstock = temptotalstock;
            p.clossingstock = tempclossingstock;
            return p;
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
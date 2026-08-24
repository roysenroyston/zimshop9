using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopMate.ModelDto;
using ShopMate.Models;
using WebErrorLogging.Utilities;

namespace ShopMate.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string Date = "")
        {
           HomeDto home = new HomeDto();

            using (SIContext db = new SIContext())
            {
                DateTime tDate = DateTime.Now;
                if (Date.Length > 2)
                {
                    tDate = Convert.ToDateTime(Date);
                }
                try
                {


                    int warehouse = int.Parse(Env.GetUserInfo("WarehouseId"));
                    int tDay = tDate.Day;
                    int tMonth = tDate.Month;
                    int tYear = tDate.Year;

                    ViewBag.dates = tDate.ToString("MM/dd/yyyy");

                    home.Role = db.Roles.Count();

                    home.User = db.Users.Where(i => i.Role_RoleId.RoleName == "Admin" || i.Role_RoleId.RoleName == "User" && i.WarehouseId == warehouse).Count();
                    //home.User = db.SaleOrders.Where(i => i.Role_RoleId.RoleName == "Admin" || i.Role_RoleId.RoleName == "User" && i.WarehouseId == warehouse).Count();
                    home.Customer = db.Users.Where(i => i.Role_RoleId.RoleName == "Customer" && i.WarehouseId == warehouse).Count();
                    home.Vendor = db.Users.Where(i => i.Role_RoleId.RoleName == "Vendor" && i.WarehouseId == warehouse).Count();
                    home.todaySaleOrders = db.SaleOrders.Where(i => (i.DateAdded.Day == tDay && i.DateAdded.Month == tMonth && i.DateAdded.Year == tYear) && i.WarehouseId == warehouse).Count();
                    home.todayProcessedSaleOrders = db.SaleOrders.Where(i => (i.DateModified.Day == tDay && i.DateModified.Month == tMonth && i.DateModified.Year == tYear) && i.WarehouseId == warehouse && i.IsProcessed == true).Count();

                    var todayPurchaseCount = db.Purchases.Where(i => (i.DateAdded.Day == tDay && i.DateAdded.Month == tMonth && i.DateAdded.Year == tYear) && i.WarehouseId == warehouse).Select(i => new { i.Quantity }).ToArray();
                    if (todayPurchaseCount.Count() > 0)
                    {
                        home.todayPurchaseCount = todayPurchaseCount.Sum(i => i.Quantity);
                    }
                    //var todaySaleOrders = db.SaleOrders.Where(i => (i.DateAdded.Day == tDay && i.DateAdded.Month == tMonth && i.DateAdded.Year == tYear) && i.WarehouseId == warehouse);
                    //if (todaySaleOrders.Count() > 0)
                    //{
                    //    home.todaySaleOrders = todaySaleOrders.Count(); 
                    //}
                    //var todayProcessedSaleOrders = db.SaleOrders.Where(i => (i.DueDate.Day == tDay && i.DueDate.Month == tMonth && i.DueDate.Year == tYear) && i.WarehouseId == warehouse);
                    //if (todayProcessedSaleOrders.Count() > 0)
                    //{
                    //    //home.todayProcessedSaleOrders = todayProcessedSaleOrders.Sum(i => i.SaleOrders);norlin
                    //}

                    var todaySaleCount = db.Sales.Where(i => (i.DateAdded.Value.Day == tDay && i.DateAdded.Value.Month == tMonth && i.DateAdded.Value.Year == tYear) && i.WarehouseId == warehouse).Select(i => new { i.Quantity }).ToArray();
                    if (todaySaleCount.Count() > 0)
                    {
                        home.todaySaleCount = todaySaleCount.Sum(i => i.Quantity);
                    }

                    var todayPurchaseReturnCount = db.ProductStocks.Where(i => (i.DateAdded.Value.Day == tDay && i.DateAdded.Value.Month == tMonth && i.DateAdded.Value.Year == tYear) && i.InventoryTypeId == 3 && i.WarehouseId == warehouse).Select(i => new { i.Quantity }).ToArray();
                    if (todayPurchaseReturnCount.Count() > 0)
                    {
                        home.todayPurchaseReturnCount = todayPurchaseReturnCount.Sum(i => i.Quantity);
                    }

                    var todaySaleReturnCount = db.ProductStocks.Where(i => (i.DateAdded.Value.Day == tDay && i.DateAdded.Value.Month == tMonth && i.DateAdded.Value.Year == tYear) && i.InventoryTypeId == 4 && i.WarehouseId == warehouse).Select(i => new { i.Quantity }).ToArray();
                    if (todaySaleReturnCount.Count() > 0)
                    {
                        home.todaySaleReturnCount = todaySaleReturnCount.Sum(i => i.Quantity);
                    }


                    var todayPurchaseCostSum = db.ProductStocks.Where(i => (i.DateAdded.Value.Day == tDay && i.DateAdded.Value.Month == tMonth && i.DateAdded.Value.Year == tYear) && i.InventoryTypeId == 1 && i.WarehouseId == warehouse).Select(i => new { amt = i.TotalPurchaseAmount + i.TaxAmount }).ToArray();
                    if (todayPurchaseCostSum.Count() > 0)
                    {
                        home.todayPurchaseCostSum = todayPurchaseCostSum.Sum(i => i.amt);
                    }

                    var todaySaleCostSum = db.ProductStocks.Where(i => (i.DateAdded.Value.Day == tDay && i.DateAdded.Value.Month == tMonth && i.DateAdded.Value.Year == tYear) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse).Select(i => new { i.TotalSaleAmountWithTax }).ToArray();
                    if (todaySaleCostSum.Count() > 0)
                    {
                        home.todaySaleCostSum = todaySaleCostSum.Sum(i => i.TotalSaleAmountWithTax);
                    }

                    var todayProfit = db.ProductStocks.Where(i => (i.DateAdded.Value.Day == tDay && i.DateAdded.Value.Month == tMonth && i.DateAdded.Value.Year == tYear) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse).Select(i => new { i.Profit }).ToArray();
                    if (todayProfit.Count() > 0)
                    {
                        home.todayProfit = todayProfit.Sum(i => i.Profit);
                    }

                    var todayProfitWithTax = db.ProductStocks.Where(i => (i.DateAdded.Value.Day == tDay && i.DateAdded.Value.Month == tMonth && i.DateAdded.Value.Year == tYear) && i.InventoryTypeId == 2 && i.WarehouseId == warehouse).Select(i => new { i.ProfitWithTax }).ToArray();

                    if (todayProfitWithTax.Count() > 0)
                    {
                        home.todayProfitWithTax = todayProfitWithTax.Sum(i => i.ProfitWithTax);
                    }

                    var PurchaseItemsQuantity = db.ProductStocks.Where(i => i.InventoryTypeId == 1 && i.WarehouseId == warehouse).Select(i => new { i.Quantity }).ToArray();
                    if (PurchaseItemsQuantity.Count() > 0)
                    {
                        home.PurchaseItemsQuantity = PurchaseItemsQuantity.Sum(i => i.Quantity);
                    }

                    var SaleItemsQuantity = db.ProductStocks.Where(i => i.InventoryTypeId == 2 && i.WarehouseId == warehouse).Select(i => new { i.Quantity }).ToArray();
                    if (SaleItemsQuantity.Count() > 0)
                    {
                        home.SaleItemsQuantity = SaleItemsQuantity.Sum(i => i.Quantity);
                    }
                    var stocksadjustedItemsQuantity = db.ProductStocks.Where(i => i.InventoryTypeId == 6 && i.WarehouseId == warehouse).Select(i => new { i.Quantity }).ToArray();
                    if (stocksadjustedItemsQuantity.Count() > 0)
                    {
                        home.stocksadjustedItemsQuantity = stocksadjustedItemsQuantity.Sum(i => i.Quantity);
                    }

                    var expense = db.Expenses.Where(i => (i.DateAdded.Value.Day == tDay && i.DateAdded.Value.Month == tMonth && i.DateAdded.Value.Year == tYear) && i.WarehouseId == warehouse).Select(i => new { i.Amount }).ToArray();

                    var due = db.DuePayments.Where(i => (i.DateAdded.Value.Day == tDay && i.DateAdded.Value.Month == tMonth && i.DateAdded.Value.Year == tYear) && i.WarehouseId == warehouse).Select(i => new { i.DueAmount, i.IsReturn }).ToArray();

                    home.Expense = expense.Sum(i => i.Amount);
                    home.DueGiven = due.Where(i => i.IsReturn == false).Sum(i => i.DueAmount);
                    home.DueReturn = due.Where(i => i.IsReturn == true).Sum(i => i.DueAmount);
                }
                catch (Exception ex)
                {
                    Helper.WriteError(ex, ex.Message);
                }
            }

            return View(home);

        }


        public class DateTable
        {
            public DateTime DateAdded { get; set; }
        }


        public JsonResult LineChart(int lastDay)
        {


            // forgot above all code if you bind this line chart form your database table

            List<GraphData> dataList = new List<GraphData>();

            var LastDays = DateTime.Now.Date.AddDays(-lastDay);
            SIContext db = new SIContext();
            ///listDateTable just add your table where have date field like db.User
            var LastRegister = db.Sales.Where(i => i.DateAdded >= LastDays).ToArray();

            for (int i = 0; i < lastDay; i++)
            {
                var dateDynamic = DateTime.Now.Date.AddDays(-i);
                int year = dateDynamic.Year;
                int month = dateDynamic.Month;
                int day = dateDynamic.Day;

                DateTime newDate = new DateTime(year, month, day);
                var hav = LastRegister.Where(j => j.DateAdded.Value.Date == newDate.Date);
                if (hav.Count() > 0)
                {
                    GraphData gdata = new GraphData();
                    gdata.label = newDate.ToString("yyyy-MM-dd");
                    gdata.value = hav.Sum(k => k.Quantity);
                    dataList.Add(gdata);
                }
                else
                {
                    GraphData gdata = new GraphData();
                    gdata.label = newDate.ToString("yyyy-MM-dd");
                    gdata.value = 0;
                    dataList.Add(gdata);
                }

            }

            return Json(dataList, JsonRequestBehavior.AllowGet);
        }
        private class GraphData
        {
            public string label { get; set; }
            public decimal value { get; set; }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}

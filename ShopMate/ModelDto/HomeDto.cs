using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.ModelDto
{
    public class HomeDto
    {
        //public int User { get; set; }
        //public int SalesOrder { get; set; }

        //public int Role { get; set; }
        //public int Menu { get; set; } 
        //public int Customer { get; set; } 
        //public int Vendor { get; set; }

        //public decimal PurchaseCount { get; set; }
        //public decimal todayPurchaseCount { get; set; }
        //public decimal todaySaleCount { get; set; }
        //public decimal? taxAmount { get; set; }
        //public decimal stocksadjustedItemsQuantity { get; set; }

        //public decimal todayPurchaseCostSum { get; set; }
        //public decimal todaySaleCostSum { get; set; }
        //public decimal todayrtgssum { get; set; }
        //public decimal todayProfit { get; set; }
        //public decimal todayProfitWithTax { get; set; }

        //public decimal PurchaseItemsQuantity { get; set; }
        //public decimal SaleItemsQuantity { get; set; }
        //public decimal WarestockRemaining { get; set; }

        //public decimal todayPurchaseReturnCount { get; set; }
        //public decimal todaySaleReturnCount { get; set; }
        //public int todaySaleOrders { get; set; }
        //public int TotalExpiryAlerts { get; set; }
        //public int todayProcessedOrders { get; set; }
        //public int todayProcessedSaleOrders { get; set; }



        //public decimal Expense { get; set; }
        //public decimal DueGiven { get; set; }
        //public decimal DueReturn { get; set; }
        public int User { get; set; }
        public int SalesOrder { get; set; }

        public int Role { get; set; }
        public int Menu { get; set; }
        public int Customer { get; set; }
        public int Vendor { get; set; }

        public decimal todayPurchaseCount { get; set; }
        public decimal todaySaleCount { get; set; }
        public decimal stocksadjustedItemsQuantity { get; set; }

        public decimal todayPurchaseCostSum { get; set; }
        public decimal todayPurchasedRawMaterialsCostSum { get; set; }
        public decimal todaySaleCostSum { get; set; }
        public decimal todaySaleCostSum1 { get; set; }
        public decimal todayProfit { get; set; }
        public decimal todayProfitWithTax { get; set; }
        public decimal Quantity { get; set; }
        public decimal? todayvat { get; set; }
        public decimal? todayvat1 { get; set; }
        public decimal PurchaseItemsQuantity { get; set; }
        public decimal SaleItemsQuantity { get; set; }
        public decimal WarestockRemaining { get; set; }
        public decimal RawMaterialsItemsQuantity { get; set; }
        public decimal todayPurchaseReturnCount { get; set; }
        public decimal todaySaleReturnCount { get; set; }
        public int todaySaleOrders { get; set; }
        public int todayProcessedOrders { get; set; }
        public int todayPendingOrders { get; set; }
        public int todayProcessedSaleOrders { get; set; }

        public string companyname { get; set; }

        public decimal Expense { get; set; }
        public decimal DueGiven { get; set; }
        public decimal DueReturn { get; set; }
    }
   
}

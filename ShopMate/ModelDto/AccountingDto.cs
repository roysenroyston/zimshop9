using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.ModelDto
{
    public class StockDto
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal purchaseprice { get; set; }
        public decimal saleprice { get; set; }
        public decimal cost { get; set; }
        public decimal Amount { get; set; }
        
    }
    public class ProfitorlossDto
    {
       
        public decimal NetProfit { get; set; }
        public decimal ProfitWithTax { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal RetainedProfit { get; set; }
        public decimal Saless{ get; set; }
        public decimal otherincomes { get; set; }
        public decimal salesreturns { get; set; }
        public decimal purchases{ get; set; }
        public decimal purchasesreturns { get; set; }
        public decimal expenses { get; set; }
        public decimal dividends{ get; set; }
        public decimal interimdividends { get; set; }
        public decimal openingstock { get; set; }
        public decimal clossingstock { get; set; }
        public decimal Tax { get; set; }


    }

  

}
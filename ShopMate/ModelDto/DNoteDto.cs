using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.ModelDto
{

    public class DNoteDto
    {

        public int Id { get; set; }


        public List<DNoteMaterialDto> items { get; set; }


        public int? invoiceNo { get; set; }

        public int? OrderNo { get; set; }


        public string CustomerUser { get; set; }

        public bool delivered { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContact { get; set; }
        public string CompanyName { get; set; }
        public Nullable<DateTime> ddate { get; set; }
        public string ToInfo { get; set; }
        public string Logo { get; set; }
        public int InvoiceId { get; set; }

    }
    public class DNoteMaterialDto
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public string Description { get; set; }
    }
    public class VanSaleDto
    {

        public int Id { get; set; }

        public DateTime DateAdded{ get; set; }
        public string Van { get; set; }
        public string Driver { get; set; }
        public string Warehouse { get; set; }
        public decimal StockValue { get; set; }
        public decimal StockValueRtgs { get; set; }
        public string Route { get; set; }

        public List<VanSaleItemDto> items { get; set; }


    }
    public class VanSaleItemDto
    {

        public int? Id { get; set; }
        public string Product { get; set; }
        public int ClosingStock { get; set; }
        public int OpeningStock { get; set; }
        public decimal SalePrice { get; set; }
        public decimal SalePriceRtgs { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? StockAmount { get; set; }
        public decimal? StockValue { get; set; }
        public decimal? StockValueRtgs { get; set; }
        public decimal GP { get; set; }
        public decimal OverallGP { get; set; }
        public decimal? Sales { get; set; }        
        public int GoodsSold { get; set; }
        public DateTime DateAdded { get; set; }
        public int? VanSaleId { get; set; }
        public string Route { get; set; }
  
    }



}

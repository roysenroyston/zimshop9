using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class VanSaleItem
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Product")]
        public int? ProductId { get; set; }
        public virtual Product Product_ProductId { get; set; }

       

        [DisplayName("Description")]
        public string Description { get; set; }

        public int? ClosingStock { get; set; }

        public int OpeningStock { get; set; }

        [Required]
        [DisplayName("Sale Price")]
        public Decimal SalePrice { get; set; }
        [Required]
        [DisplayName("Sale Price")]
        public Decimal SalePriceRtgs { get; set; }
        

        [Required]
        [DisplayName("Cost Price")]
        public Decimal UnitPrice { get; set; }

        [Required]
        [DisplayName("Stock Amount")]
        public decimal? StockAmount { get; set; }

        [DisplayName("Stock Value")]
        public decimal? StockValue { get; set; }
        [DisplayName("Stock Value (ZWL)")]
        public decimal? StockValueRtgs { get; set; }

        [DisplayName("Gross Profit")]
        public Decimal GP { get; set; }

        [DisplayName("Sales")]
        public decimal Sales { get; set; }

        [DisplayName("Goods Sold")]
        public int GoodsSold { get; set; }

        [Required]
        [DisplayName("Date Added")]
        public DateTime DateAdded { get; set; }

        [DisplayName("Van Sale Id")]
        public int? VanSaleId { get; set; }
        public virtual VanSale VanSale_VanSaleId { get; set; }
        public decimal? OverallGP { get; set; }
        public bool IsReturned { get; set; }

    }
}
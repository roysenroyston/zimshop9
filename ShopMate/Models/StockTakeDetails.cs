using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class StockTakeDetails
    {
        [DisplayName(" ID")]
        public int Id { get; set; }

        [Required]
        [DisplayName("Product")]
        public int? ProductId { get; set; }
        public virtual Product Product_ProductId { get; set; }

        public string productName { get; set; }
        [Required]
        [DisplayName("Actual Quantity")]
        public decimal actualquantity { get; set; }
        [Required]
        [DisplayName("Counted")]
        public decimal counted { get; set; }
        [Required]
        [DisplayName("Actual Value")]
        public decimal actualvalue { get; set; }
        [Required]
        [DisplayName("Counted Value")]
        public decimal countedvalue { get; set; }
        [DisplayName("Variance")]
        public decimal variance { get; set; }
        [DisplayName("Variance Value")]
        public decimal variancevalue { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }
        [DisplayName("Date Added")]
        public Nullable<DateTime> DateAdded { get; set; }
        [Required]
        [DisplayName("Stock Take")]
        public int? StockTakeId { get; set; }
        public virtual Product StockTake_StockTakeId { get; set; }
    }
}
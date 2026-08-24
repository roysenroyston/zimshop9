using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class StockTake
    {
        [DisplayName(" ID")]
        public int Id { get; set; }

        //[DisplayName("Product Category")]
        //public int? ProductCategoryId { get; set; }
        //public virtual ProductCategory ProductCategory_ProductCategoryId { get; set; }
        //[Required]
        //[DisplayName("Product")]
        //public int? ProductId { get; set; }
        //public virtual Product Product_ProductId { get; set; }
        //[Required]
        //[DisplayName("Actual Quantity")]
        //public decimal actualquantity { get; set; }
       
        //[DisplayName("Counted")]
        //public decimal counted { get; set; }
        public int addedby { get; set; }
        //[DisplayName("Variance")]
        //public decimal variancevalue { get; set; }
        [DisplayName("Date Added")]
        public Nullable<DateTime> DateAdded { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }

    }
}
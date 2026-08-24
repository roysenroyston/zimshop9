using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
    {
        [TrackChanges]
        public class SaleOrderItem
        {
            [DisplayName("S.No")]
            public int Id { get; set; }
            [Required]
            [DisplayName("Product")]
            public int? ProductId { get; set; }
            public virtual Product Product_ProductId { get; set; }         
            [Required]
            [DisplayName("Quantity")]
            public Decimal Quantity { get; set; }
            [Required]
            [DisplayName("Sale Price")]
            public Decimal SalePrice { get; set; }
            [Required]
            [DisplayName("Tax Amount")]
            public Decimal TaxAmount { get; set; }
            [Required]
            [DisplayName("Total Amount")]
            public Decimal TotalAmount { get; set; }
            [Required]
            [DisplayName("Total Amount With Tax")]
            public Decimal TotalAmountWithTax { get; set; }
            //[DisplayName("Added By")]
            //public Nullable<int> AddedBy { get; set; }
            [DisplayName("Date Added")]
            public Nullable<DateTime> DateAdded { get; set; }
            [DisplayName("Tax")]
            public Nullable<int> TaxId { get; set; }
            //[DisplayName("Purchase")]
            //public Nullable<int> PurchaseId { get; set; }
            [DisplayName("Sale Order Id")]
            public Nullable<int> SaleOrderId { get; set; }
            //[Required]
            //[DisplayName("Product Stock")]
            //public int ProductStockId { get; set; }
            //[Required]
            //[DisplayName("Transaction")]
            //public int TransactionId { get; set; }
            //[Required]
            //[DisplayName("Warehouse")]
            //public int WarehouseId { get; set; }

        }
    }



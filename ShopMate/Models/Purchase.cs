using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Purchase
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [Required]
        [DisplayName("Vendor User")]
        public int? VendorUserId { get; set; }
        public virtual Vendor Vendor_Id { get; set; }
        [Required]
        [DisplayName("Product")]
        public int? ProductId { get; set; }
        public virtual Product Product_ProductId { get; set; }
        [Required]
        [DisplayName("Quantity")]
        public Decimal Quantity { get; set; }
        [DisplayName("Returned Quantity")]
        public Decimal ReturnedQuantity { get; set; }

        [DisplayName("Invoice Number")]
        public string InvoiceNumber { get; set;}


            [Required]
        [DisplayName("Unit Price")]
        public Decimal UnitPrice { get; set; }
        [Required]
        [DisplayName("Total Amount")]
        public Decimal TotalAmount { get; set; }
        [DisplayName("Total Amount With Tax")]
        public Nullable<Decimal> TotalAmountWithTax { get; set; }
        [Required]
        [DisplayName("Date Added")]
        public DateTime DateAdded { get; set; }
        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }

        [DisplayName("Inventory Type")]
        public int? InventoryTypeId { get; set; }

        public decimal TaxAmount { get; set; }

    }
}

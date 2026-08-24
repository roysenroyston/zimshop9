using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class InvoiceItems
    {
        [DisplayName("S.No")] 
        public int Id { get; set; }
        [Required]
        [DisplayName("Product")] 
        public int? ProductId { get; set; }
        public virtual Product Product_ProductId { get; set; }
        
        [DisplayName("Invoice")]
        public Nullable<int> InvoiceId { get; set; }
        //public virtual Invoice Invoice_InvoiceId { get; set; }
        [DisplayName("Invoice")]
        public Nullable<int> InformalInvoiceId { get; set; }
        //public virtual Invoice InformalInvoice_InvoiceId { get; set; }

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
        [DisplayName("Added By")] 
        public Nullable<int> AddedBy { get; set; }
        [DisplayName("Date Added")] 
        public Nullable<DateTime> DateAdded { get; set; }
        [DisplayName("Tax")] 
        public Nullable<int> TaxId { get; set; }
        [DisplayName("Purchase")] 
        public Nullable<int> PurchaseId { get; set; }
        [DisplayName("Sale")] 
        public Nullable<int> SaleId { get; set; }
        [Required]
        [DisplayName("Product Stock")] 
        public int ProductStockId { get; set; }
        [Required]
        [DisplayName("Transaction")] 
        public int TransactionId { get; set; }
        [Required]
        [DisplayName("Warehouse")] 
        public int WarehouseId { get; set; }

        [DisplayName("Product Batch")]
        public int BatchId { get; set; }
        public decimal discount { get; set; }
    }
}

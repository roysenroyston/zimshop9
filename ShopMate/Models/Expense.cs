using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Expense
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [DisplayName("Vendor User")]
        public int? VendorUserId { get; set; }
        public virtual User User_VendorUserId { get; set; }
        [Required]
        [StringLength(200)]
        [DisplayName("Remarks")]
        public string Remarks { get; set; }
        [Required]
        [DisplayName("Amount")]
        public Decimal Amount { get; set; }
        [DisplayName("SubTotal")]
        public Decimal SubTotal { get; set; }
        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        [DisplayName("Date Added")]
        public Nullable<DateTime> DateAdded { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }
        [DisplayName("Expense")]
        public int? ExpenseId { get; set; }
        [DisplayName("Invoice Number")]
        public string InvoiceNumber { get; set; }
        [DisplayName("Vat Number")]
        public string VatNumber { get; set; }
        [DisplayName("Tax Amount")]
        public Decimal TaxAmount { get; set; } 
        [DisplayName("Invoice Date")]
        public string InvoiceDate { get; set; }

        [DisplayName("Vendor Name")]
        public int? Vendorname { get; set; }
        //[Required]
        //[DisplayName("Currency")]
        //public int CurrencyId { get; set; }
        //[DisplayName("Currency Amount")]
        //public Decimal CurrencyAmount { get; set; }
        //[DisplayName("Currency Tax Amount")]
        //public Decimal CurrencyTaxAmount { get; set; }

    }
}

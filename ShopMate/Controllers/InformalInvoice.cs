using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
   
        [TrackChanges]
        public class InformalInvoice
        {
            [DisplayName("S.No")]
            public int Id { get; set; }
            [Required]
            [DisplayName("Is Billed")]
            public bool IsBilled { get; set; }
            [Required]
            [DisplayName("User")]
            public int UserId { get; set; }

            [DisplayName("Invoice Number")]
            public Nullable<int> InvoiceNo { get; set; }

            [DisplayName("Job Number")]
            public Nullable<int> ProjectNumber { get; set; }

            [DisplayName("Order Number")]
            public Nullable<int> orderNumber { get; set; }

            [DisplayName("Total")]
            public Nullable<decimal> total { get; set; }
            [DisplayName("Discount")]
            public Nullable<decimal> payment { get; set; }
            [DisplayName("Due")]
            public Nullable<decimal> balance { get; set; }

            [DisplayName("VAT")]
            public Nullable<decimal> vat { get; set; }

            [DisplayName("Subtotal")]
            public Nullable<decimal> subtotal { get; set; }
            [DisplayName(" Due Date ")]
            public Nullable<DateTime> Duedate { get; set; }
            [DisplayName("Customer Vat Reg")]
            public string CustomerVatReg { get; set; }

            public string IsPurchaseOrSale { get; set; }
            [DisplayName("Added By")]
            public Nullable<int> AddedBy { get; set; }
            [DisplayName("Date Added")]
            public Nullable<DateTime> DateAdded { get; set; }
            [DisplayName("Date Modied")]
            public Nullable<DateTime> DateModied { get; set; }
            [DisplayName("Modified By")]
            public Nullable<int> ModifiedBy { get; set; }
            [Required]
            [DisplayName("Warehouse")]
            public int WarehouseId { get; set; }
            public Nullable<int> salesrepId { get; set; }
            public virtual ICollection<InvoiceItems> InvoiceItems_InvoiceIds { get; set; }

            public virtual ICollection<InvoiceMaterials> InvoiceMaterials_invoice { get; set; }
        public DateTime? DatePaid { get; set; }
        public Nullable<int> InvoicePaymentMethodId { get; set; }


    }
    }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShopMate.Models
{
    public class DebitCreditNote
    {
        [DisplayName("S.No")]
        public int Id { get; set; }

        [DisplayName("Is Fiscilazed")]
        public bool IsFiscilazed { get; set; }

        [DisplayName("Invoice Number")]
        public string InvoiceNo { get; set; }

        [DisplayName("Total")]
        public Nullable<decimal> total { get; set; }

        [DisplayName("VAT")]
        public Nullable<decimal> vat { get; set; }

        [DisplayName("Subtotal")]
        public Nullable<decimal> subtotal { get; set; }

        [DisplayName(" Date Added ")]
        public Nullable<DateTime> Duedate { get; set; }

        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }

        public virtual Warehouse Warehouse_WarehouseId { get; set; }

        [DisplayName("Remarks")]
        public string Remarks { get; set; }

        [DisplayName("RecieptId")]
        public int RecieptId { get; set; }

        [DisplayName("receiptNo")]
        public string receiptNo { get; set; }

        [DisplayName("ReceiptType")]
        public string ReceiptType { get; set; }

        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        [DisplayName("Receipt Currency")]
        public string ReceiptCurrency { get; set; }
        public int CustomerId { get; set; }
        [DisplayName("Error Message")]
        public string ErrorMessage { get; set; }
        [DisplayName("Status")]
        public string Status { get; set; }

    }

}
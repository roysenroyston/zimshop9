using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Paymenttrack
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("Cash ")]
        public decimal cash { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("Swipe")]
        public decimal swipe { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("Ecocash")]
        public decimal ecocash{ get; set; }
        [DefaultValue(0.00)]
        [DisplayName("One money")]
        public decimal onemoney{ get; set; }
        [DefaultValue(0.00)]
        [DisplayName("Telecash")]
        public decimal telecash{ get; set; }
        [DefaultValue(0.00)]
        [DisplayName("USD")]
        public decimal usd{ get; set; }
        [DefaultValue(0.00)]
        [DisplayName("Rand")]
        public decimal Rand { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("Zipit")]
        public decimal zipit { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("FBC")]
        public decimal fbc { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("ACL")]
        public decimal acl { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("Pula")]
        public decimal pula { get; set; }

        [DefaultValue(0.00)]
        [DisplayName("Change")]
        public decimal Change { get; set; }

        [DisplayName("Sale Id")]
        public int? SaleId { get; set; }
        public virtual Sale Sale_SaleId { get; set; }

        [DisplayName("Invoice Id")]
        public int? InvoiceId { get; set; }
        public virtual Invoice Invoice_InvoiceId { get; set; }
        [DisplayName("Date Added")]
        public Nullable<DateTime> DateAdded { get; set; }
        [DisplayName("Modified By")]
        public Nullable<int> ModifiedBy { get; set; }
        [DisplayName("Date Modied")]
        public Nullable<DateTime> DateModied { get; set; }
        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }
       
        [DisplayName("Accountpayment Id")]
        public int? AccountpaymentId { get; set; }
       
    }
}
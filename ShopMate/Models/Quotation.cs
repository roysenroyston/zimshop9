using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Quotation
    {
        [DisplayName("S.No")]
        public int Id { get; set; }

        [Required]
        [DisplayName("SubTotal")]
        public decimal SubTotal { get; set; }

        [Required]
        [DisplayName("VAT")]
        public decimal VAT { get; set; }

        [Required]
        [DisplayName("Total")]
        public decimal Total { get; set; }

        [DisplayName("Added By")]
        public int AddedBy { get; set; }
        [DisplayName("Customer")]
        public int customerId { get; set; }
        public User customer { get; set; }
        [DisplayName("IssueDate")]
        public DateTime IssueDate { get; set; }

        [DisplayName("ValidUntil")]
        public Nullable<DateTime> ValidUntil { get; set; }

        [DisplayName("Modified By")]
        public int ModifiedBy { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }
        public IEnumerable<QuotationItems> items { get; set; }
        public bool approved { get; set; }
    }
}
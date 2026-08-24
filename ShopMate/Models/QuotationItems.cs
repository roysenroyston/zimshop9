using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShopMate.Models
{
    [TrackChanges]
    public class QuotationItems
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [Required]
        [DisplayName("Product")]
        public int? ProductId { get; set; }
        public virtual Product Product_ProductId { get; set; }

        [Required]
        [DisplayName("Quantity")]
        public decimal Quantity { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [Required]
        [DisplayName("UnitPrice")]
        public decimal UnitPrice { get; set; }

        [Required]
        [DisplayName("TotalPrice")]
        public decimal TotalPrice { get; set; }        [DisplayName("Tax")]
        public Nullable<int> TaxId { get; set; }

        public int QuotationId { get; set; }
        public Quotation Quotation { get; set; }
    }
}
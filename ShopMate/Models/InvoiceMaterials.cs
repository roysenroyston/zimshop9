using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    [TrackChanges]
    public class InvoiceMaterials
    {

        [DisplayName(" ID")]
        public int Id { get; set; }

        [DisplayName("Description ")]
        public string description { get; set; }

        [DisplayName("Quantity")]
        [DefaultValue(0.00)]
        public decimal quantity { get; set; }

        [DisplayName("Price/Rate")]
        [DefaultValue(0.00)]
        public decimal rate { get; set; }
        [DisplayName("Discount")]
        [DefaultValue(0.00)]
        public decimal discount { get; set; }
        public string vat { get; set; }
        [DisplayName("Invoice")]
        public Nullable<int> InvoiceId { get; set; }

        public Nullable<int> InformalInvoiceId { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product_ProductId { get; set; }
    }
}
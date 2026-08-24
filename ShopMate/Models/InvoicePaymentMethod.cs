using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class InvoicePaymentMethod
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Payment Method")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Due in Days")]
        public int DueIn { get; set; }
    }
}
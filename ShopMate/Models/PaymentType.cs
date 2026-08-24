using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class PaymentType
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Payment Type")]
        public string Name { get; set; }
    }
}
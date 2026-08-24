using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class GRVMaterials
    {
        [DisplayName(" ID")]
        public int Id { get; set; }

        [Required]
        [DisplayName("Quantity")]
        public Decimal Quantity { get; set; }

        [Required]
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Product")]
        public int? ProductId { get; set; }
        public virtual Product Product_ProductId { get; set; }
        [Required]
        [DisplayName("Unit Price")]
        public Decimal UnitPrice { get; set; }

        [DisplayName("Total Price")]
        public Decimal TotalPrice { get; set; }
        [Required]
        [DisplayName("Status")]
        public string Status { get; set; }
        public GRV GRV { get; set; }

        [DisplayName("GRV Id")]
        public int? GRVId { get; set; }
        public virtual GRV GRV_GRVId { get; set; }
    }
}
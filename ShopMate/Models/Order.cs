using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Order
    {
        [DisplayName(" ID")]
        public int Id { get; set; }
        [Required]
        [DisplayName("Description")]
        public string goods { get; set; }
        [Required]
        [DisplayName("Supplier")]
        public int supplier { get; set; }
    
        [DisplayName("Order Number")]
        public Nullable<int> orderNumber { get; set; }
        [DisplayName("Order Date")]
        public Nullable<DateTime> purchasedate { get; set; }
        public IEnumerable<OrderMaterials> OrderMaterials { get; set; }
    }
}
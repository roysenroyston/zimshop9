using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    
        [TrackChanges]
      public class OrderMaterials
    {
            [DisplayName("S.No")]
            public int Id { get; set; }

            [Required]
            [DisplayName("Quantity")]
            public decimal Quantity { get; set; }

            [DisplayName("Description")]
            public string Description { get; set; }

           

            public int OrderId { get; set; }
            public Order Order { get; set; }
        }
    
}
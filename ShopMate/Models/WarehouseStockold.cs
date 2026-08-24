using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    [TrackChanges]
    public class WarehouseStock
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Product")]
        public int? ProductId { get; set; }
        public virtual Product Product_ProductId { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int? WarehouseId { get; set; }
        public virtual Warehouse Warehouse_WarehouseId { get; set; }

        public decimal RemainingQuantity { get; set; }
    }
}
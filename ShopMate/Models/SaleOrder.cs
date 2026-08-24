using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class SaleOrder
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [Required]
        [DisplayName("Customer")]
        public int? CustomerId { get; set; }
        public virtual User User_CustomerId { get; set; }

        [Required]
        [DisplayName("Date Added")]
        public DateTime DateAdded { get; set; }
        [Required]
        [DisplayName("Date Modified")]
        public DateTime DateModified { get; set; }
        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }
        public bool IsProcessed { get; set; }
        public int ModifiedBy { get; set; }
        //[DisplayName("Inventory Type")]
        //public int? InventoryTypeId { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Stores
    {
        [DisplayName(" ID")]
        public int Id { get; set; }
        [DisplayName("Total Amount")]
        public Decimal totalprice { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }
        [DisplayName("Purchase Date")]
        public Nullable<DateTime> purchasedate { get; set; }
        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        //public Nullable<int> RawMaterialId { get; set; }
        public IEnumerable<StoresMaterials> StoresMaterials { get; set; }
    }
}
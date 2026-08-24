using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class RawMaterials
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        [DisplayName("Date Added")]
        public Nullable<DateTime> DateAdded { get; set; }
        [SkipTracking]
        [Required]
        [DisplayName("Stock Alert")]
        public int StockAlert { get; set; }
        [Required]
        [SkipTracking]
        [DisplayName("Tax")]
        public Nullable<int> TaxId { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }
        [DisplayName("Remaining Quantity")]
        [DefaultValue(0.00)]
        public Decimal RemainingQuantity { get; set; }
        [DisplayName("Remaining Amount")]
        [DefaultValue(0.00)]
        public Decimal RemainingAmount { get; set; }
        public virtual ICollection<RawMaterialStock> RawMaterialStock_RawMaterialIds { get; set; }
        public virtual ICollection<Manufacturing> Manufacturing_ManufacturingIds { get; set; }
       
        public virtual ICollection<Stores> Stores_StoresIds { get; set; }
    }
}
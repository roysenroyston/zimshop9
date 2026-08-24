using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class RawMaterialStock
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [Required]
        [DisplayName("Raw Material")]
        public int? RawMaterialsId { get; set; }
        public virtual RawMaterials RawMaterials_RawMaterialsId { get; set; }
        [Required]
        [DisplayName("Quantity")]
        public Decimal Quantity { get; set; }
        [DisplayName("Purchase Price")]
        public Decimal PurchasePrice { get; set; }
        [DisplayName("Total Purchase Amount")]
        public Decimal TotalPurchaseAmount { get; set; }
        [SkipTracking]
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        [DisplayName("Date Added")]
        public Nullable<DateTime> DateAdded { get; set; }
        [Required]
        [DisplayName("Inventory Type")]
        public int? InventoryTypeId { get; set; }
        public virtual InventoryType InventoryType_InventoryTypeId { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }
    }
}
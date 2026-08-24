using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class StoresMaterials
    {
        [DisplayName(" ID")]
        public int Id { get; set; }
        [Required]
        [DisplayName("Goods")]
        public string goods { get; set; }
        [Required]
        [DisplayName("Quantity")]
        public Decimal Quantity { get; set; }
        
        [DisplayName("Unit Price")]
        public Decimal unitprice { get; set; }
        [DisplayName("Total Price")]
        public Decimal Total { get; set; }
        public Stores store { get; set; }
        
         [DisplayName("Inventory Type")]
        public int? InventoryTypeId { get; set; }
        public virtual InventoryType InventoryType_InventoryTypeId { get; set; }
        [DisplayName("Raw Material Id")]
        public int RawMaterialsId { get; set; }
        public int rawmaterialStockId { get; set;}
        public int TransactionId { get; set; }
    }
}
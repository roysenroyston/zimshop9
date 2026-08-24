using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Setting
    {
        [DisplayName("S.No")] 
        public int Id { get; set; }
        [Required]
        [StringLength(200)] 
        [DisplayName("s Key")] 
        public string sKey { get; set; }
        [Required]
        [StringLength(3000)] 
        [DisplayName("s Value")] 
        public string sValue { get; set; }
        [Required]
        [StringLength(500)] 
        [DisplayName("s Group")] 
        public string sGroup { get; set; }
        [DisplayName("Warehouse")] 
        public Nullable<int> WarehouseId { get; set; }

    }
}

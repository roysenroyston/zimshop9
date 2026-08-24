using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class Van
    {
        public int Id { get; set; }
        [DisplayName("RegNumber")]
        public string RegNumber { get; set; }
        [DisplayName("IsActive")]
        public bool IsActive { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int? WarehouseId { get; set; }
        public virtual Warehouse Warehouse_WarehouseId { get; set; }
    }

}
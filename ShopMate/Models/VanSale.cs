using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class VanSale
    {


        public int Id { get; set; }

        [DisplayName("UserId")]
        public int? UserId { get; set; }
        public virtual User User_UserId { get; set; }

        [Required]
        [DisplayName("Date Added")]
        public DateTime DateAdded { get; set; }

        [Required]
        [DisplayName("Van Name")]
        public int? VanId { get; set; }
        public virtual Van Van_VanId { get; set; }

        public string Driver { get; set; }

        public int WarehouseId { get; set; }

        public decimal StockValue { get; set; }

        public decimal StockValueRtgs { get; set; }

        public bool IsCanceled { get; set; }


        public bool IsReturned { get; set; }
        [DisplayName("Route")]
        public string Route { get; set; }
        [DisplayName("approved")]
        public bool approved { get; set; }

    }
}
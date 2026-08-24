using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class FinishedManufacturedItem
    {
        [DisplayName(" ID")]
        public int Id { get; set; }
        public virtual Manufacturing Manufacturing { get; set; }
        public virtual finishedItem finishedItem { get; set; }
        public int ManufacturingId { get; set; }

        public int finishedItemId { get; set; }
        [Required]
        [DisplayName("Quantity")]
        public Decimal Quantity { get; set; }

      

    }
}
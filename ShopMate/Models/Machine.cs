using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Machine
    {
        [DisplayName("Machine ID")]
        public int Id { get; set; }
        [Required]
        [DisplayName("Machine Name")]
        public string Name { get; set; }
        [Required]
        [DisplayName("Workshop")]
        public Nullable<int> WarehouseId { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("Value")]
        public decimal Cost { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("Hourly Rate")]
        public decimal rate { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("Depreciation")]
        public decimal depreciation { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Tax
    {
        [DisplayName("S.No")] 
        public int Id { get; set; }
        [Required]
        [StringLength(100)] 
        [DisplayName("Name")] 
        public string Name { get; set; }
        [Required]
        [DisplayName("Tax Rate")] 
        public Decimal TaxRate { get; set; }
        [StringLength(100)] 
        [DisplayName("Other")] 
        public string Other { get; set; }

    }
}

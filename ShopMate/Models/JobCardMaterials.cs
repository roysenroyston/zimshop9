using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    [TrackChanges]
    public class JobCardMaterials
    {
        [DisplayName(" ID")]
        public int Id { get; set; }

        [DisplayName("Material")]
        public string material { get; set; }


        [DisplayName("Quantity")]
        public Decimal Quantity { get; set; }

        [DisplayName("Price")]
        public decimal price { get; set; }

        public JobCard JobCard { get; set; }
    }
}
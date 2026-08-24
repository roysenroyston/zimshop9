using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class DNoteMaterial
    {
        [DisplayName(" ID")]
        public int Id { get; set; }
        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Quantity")]
        public Decimal Quantity { get; set; }

        public int DNoteId { get; set; }
        public DeliveryNote DNote { get; set; }
    }
}
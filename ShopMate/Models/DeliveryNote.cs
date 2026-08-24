using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class DeliveryNote
    {


        [DisplayName("Delivery Note No")]
        public int Id { get; set; }

        public IEnumerable<DNoteMaterial> DNoteMaterials { get; set; }


        [Required]
        [DisplayName("Invoice No")]
        public int? invoiceNo { get; set; }
        [Required]
        [DisplayName("Order No")]
        public int? OrderNo { get; set; }

        [DisplayName("Customer")]
        public int CustomerUserId { get; set; }

        [DisplayName("Delivered")]
        public bool delivered { get; set; }
        [DisplayName("Date Delivered")]
        public Nullable<DateTime> ddate { get; set; }


    }
}
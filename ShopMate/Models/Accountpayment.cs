using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Accountpayment
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [Required]
        [DisplayName("User")]
        public int? UserId { get; set; }
        public virtual User User_UserId { get; set; }
        [Required]
        [DisplayName("Amount")]
        public Decimal Amount { get; set; }
        [Required]
        [StringLength(200)]
        [DisplayName("Remarks")]
        public string Remarks { get; set; }
        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        [DisplayName("Date Added")]
        public Nullable<DateTime> DateAdded { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }

        [DisplayName("Is Return")]
        public bool IsReturn { get; set; }
        public decimal cash { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("Swipe")]
        public decimal swipe { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("ecocash")]
        public decimal ecocash { get; set; }
        [DefaultValue(0.00)]
        [DisplayName("Change")]
        public decimal Change { get; set; }

    }
}

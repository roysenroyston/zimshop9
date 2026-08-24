using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class DuePayment
    {
        [DisplayName("S.No")] 
        public int Id { get; set; }
        [Required]
        [DisplayName("User")] 
        public int? UserId { get; set; }
        public virtual User User_UserId { get; set; }
        [Required]
        [DisplayName("Due Amount")] 
        public Decimal DueAmount { get; set; }
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

    }
}

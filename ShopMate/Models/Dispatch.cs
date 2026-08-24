using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class Dispatch
    {
        [DisplayName("Dispatch No")]
        public int Id { get; set; }
       
        [DisplayName("Dispatched To")]
        public string DispatchTo { get; set; }
        [Required]
        [DisplayName("Invoice No")]
        public int? invoiceNo { get; set; }
        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }
        [DisplayName("Date Added")]
        public Nullable<DateTime> DateAdded { get; set; }


    }
}
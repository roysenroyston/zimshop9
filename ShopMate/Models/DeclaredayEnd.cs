using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    [TrackChanges]
    public class DeclaredayEnd
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [Required]
        [DisplayName("Declare")]
        public bool Declared { get; set; }
        [Required]
        [DisplayName("User")]
        public int UserId { get; set; }
       
      
        [DisplayName("2 ZWG")]
        public Nullable<int> twobond { get; set; }
        
       
        [DisplayName("5 ZWG")]
        public Nullable<int> fivebond { get; set; }
      
        
        [DisplayName("10 ZWG")]
        public Nullable<int> tenbond { get; set; }
        
        
        [DisplayName("20 ZWG")]
        public Nullable<int> twentybond { get; set; }

        [DisplayName("50 ZWG")]
        public Nullable<int> fiftybond { get; set; }
        [DisplayName("100 ZWG")]
        public Nullable<int> hundredbond { get; set; }
        [DisplayName("1 Dollar")]
        public Nullable<int> onedollars { get; set; }
        
       
        [DisplayName("2 Dollars")]
        public Nullable<int> twodollars { get; set; }
        
        
        [DisplayName("5 Dolars")]
        public Nullable<int> fiveDollars { get; set; }
        
      
        [DisplayName("10 Dollars")]
        public Nullable<int> tenDollars { get; set; }
        
       
        [DisplayName("20 Dollars")]
        public Nullable<int> twentydollars { get; set; }
       
      
        [DisplayName("50 Dollars")]
        public Nullable<int> fiftyDollars { get; set; }
        
        
        [DisplayName("100 Dollars")]
        public Nullable<int> hundreddollars { get; set; }
        [DisplayName("Cash Bond")]
        public Nullable<decimal> totalcash { get; set; }
        [DisplayName("EcoCash")]
        public Nullable<decimal> ecocash { get; set; }
        [DisplayName("TeleCash ")]
        public Nullable<decimal> telecash { get; set; }
        [DisplayName("One Money ")]
        public Nullable<decimal> onemoney { get; set; }
        [DisplayName("RTGS Swipe")]
        public Nullable<decimal> rtgs { get; set; }
        [DisplayName("FBC Swipe")]
        public Nullable<decimal> Fbc { get; set; }
        [DisplayName("ZIPIT Swipe")]
        public Nullable<decimal> Zipit { get; set; }
        [DisplayName("ACL Swipe")]
        public Nullable<decimal> Acl { get; set; }
        [DisplayName("FCA ")]
        public Nullable<decimal> nostro { get; set; }
        [DisplayName("Total")]
        public Nullable<decimal> totalAmount { get; set; }
        [DisplayName("Total Change")]
        public Nullable<decimal> totalChange { get; set; }
        [DisplayName("Total CashUsd")]
        public Nullable<decimal> totalCashUsd { get; set; }
        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        [DisplayName("Date Added")]
        public Nullable<DateTime> DateAdded { get; set; }
        [DisplayName("Date Modied")]
        public Nullable<DateTime> DateModied { get; set; }
        [DisplayName("Modified By")]
        public Nullable<int> ModifiedBy { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }
       // public virtual ICollection<DeclaredayEnd> DayEnddeclare_Ids { get; set; }
    }
}
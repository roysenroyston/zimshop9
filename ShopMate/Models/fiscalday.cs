using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class fiscalday
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [Required]
  
        [DisplayName("DeviceId")]
        public int DeviceId { get; set; }

        [DisplayName("Fiscal Day No")]
        public int FiscalDayNo { get; set; }
     
        [SkipTracking]
        [StringLength(100)]
        [DisplayName("Operation Id")]
        public string OperationId { get; set; }

        [DisplayName("Fiscal Status")]
        public string FiscalStatus { get; set; }
        [Required]
        [DisplayName("Is Open")]
        public Boolean IsOpen { get; set; }
        [SkipTracking]
        [DisplayName("Date Opened")]
        public Nullable<DateTime> DateOpened { get; set; }
        [SkipTracking]
        [DisplayName("Date Closed")]
        public Nullable<DateTime> DateClosed { get; set; }
        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        [DisplayName("WarehouseId")]
        public int WarehouseId { get; set; }
   

    }
}
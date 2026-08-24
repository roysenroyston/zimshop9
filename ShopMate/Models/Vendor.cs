using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class Vendor
    {
        [DisplayName("S.No")]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [StringLength(111)]
        [DisplayName("Full Name")]
        public string FullName { get; set; }

        [StringLength(15)]
        [SkipTracking]
        [DisplayName("Mobile")]
        public string Mobile { get; set; }

        [StringLength(200)]
        [SkipTracking]
        [DisplayName("Email")]
        public string Email { get; set; }

        [StringLength(200)]
        [SkipTracking]
        [DisplayName("Address")]
        public string Address { get; set; }

        [SkipTracking]
        [DisplayName("About")]
        public string About { get; set; }

        [DisplayName("Join Date")]
        public Nullable<DateTime> JoinDate { get; set; }

        [DisplayName("Is Active")]
        [SkipTracking]
        public Nullable<bool> IsActive { get; set; }

        [StringLength(150)]
        [SkipTracking]
        [DisplayName("VAT Number")]
        public string vatNumber { get; set; }

        [DisplayName("Warehouse")]
        public Nullable<int> WarehouseId { get; set; }

        public virtual ICollection<Purchase> Purchase_VendorUserIds { get; set; }
    }
}
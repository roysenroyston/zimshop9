using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShopMate.Models
{
    [TrackChanges]
    public class User
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Password")]
        public string Password { get; set; }
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
        [Required]
        [DisplayName("Role")]
        public int? RoleId { get; set; }
        public virtual Role Role_RoleId { get; set; }
        [DisplayName("Join Date")]
        public Nullable<DateTime> JoinDate { get; set; }
        [DisplayName("Is Active")]
        [SkipTracking]
        public Nullable<bool> IsActive { get; set; }

        [DisplayName("Can Order")]
        [SkipTracking]
        public Nullable<bool> CanOrder { get; set; }
        [Required]
        [DisplayName("Can Login")]
        [SkipTracking]
        public bool CanLogin { get; set; }
        [StringLength(150)]
        [SkipTracking]
        [DisplayName("VAT Number")]
        public string vatNumber { get; set; }
        [DisplayName("Warehouse")]
        public Nullable<int> WarehouseId { get; set; }
        [DisplayName("Credit")]
        [DefaultValue(0.00)]
        public decimal credit { get; set; }
        [DefaultValue("")]
        public string printerMacAddress { get; set; }
        public virtual ICollection<MenuPermission> MenuPermission_UserIds { get; set; }
        public virtual ICollection<Sale> Sale_CustomerUserIds { get; set; }
        public virtual ICollection<Purchase> Purchase_VendorUserIds { get; set; }
        public virtual ICollection<DuePayment> DuePayment_UserIds { get; set; }

    }
}

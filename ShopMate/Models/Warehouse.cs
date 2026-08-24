using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShopMate.Models
{
    [TrackChanges]
    public class Warehouse
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [StringLength(15)]
        [DisplayName("Mobile")]
        public string Mobile { get; set; }
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
        [StringLength(50)]
        [DisplayName("Email")]

        public string Email { get; set; }
        [DisplayName("Expiry Date")]
        public string ExpiryDate { get; set; }
        public Nullable<DateTime> DateCreated { get; set; }
        public Nullable<int> NumberOfUsers { get; set; }
    }
}

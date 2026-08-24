using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class Customers
    {
        public int Id { get; set; }
        [DisplayName("Buyer Register Name")]
        public string BuyerRegisterName { get; set; }
        [DisplayName("Buyer Trade Name")]
        public string BuyerTradeName { get; set; }
        [DisplayName("Buyer TIN")]
        public string BuyerTIN { get; set; }
        [DisplayName("VAT Number")]
        public string VATNumber { get; set; }
        [DisplayName("Phone No")]
        public string PhoneNo { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Province")]

        public string Province { get; set; }
        [DisplayName("Street")]
        public string Street { get; set; }
        [DisplayName("House No")]
        public string HouseNo { get; set; }
        [DisplayName("City")]
        public string City { get; set; }
        [DisplayName("is Active")]
        public bool isActive { get; set; }
        [DisplayName("Warehouse Id")]
        public int WarehouseId { get; set; }
        [DisplayName("Joined Date")]
        public DateTime JoinedDate { get; set; }
    }
}
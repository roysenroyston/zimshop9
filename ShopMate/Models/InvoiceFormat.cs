using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShopMate.Models
{
    [TrackChanges]
    public class InvoiceFormat
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
        [DisplayName("Logo")]
        public string Logo { get; set; }
        [StringLength(500)]
        [DisplayName("Address Info")]
        public string AddressInfo { get; set; }
        [StringLength(500)]
        [DisplayName("Other Info")]
        public string OtherInfo { get; set; }
        [StringLength(500)]
        [DisplayName("Footer Info")]
        public string FooterInfo { get; set; }
        [DisplayName("Warehouse")]
        public Nullable<int> WarehouseId { get; set; }
        [StringLength(100)]
        [DisplayName("Company Vat Number")]
        public string VatNumber { get; set; }
        [StringLength(100)]
        [DisplayName("Company BP Number")]
        public string BPNumber { get; set; }
        [DisplayName("taxPayerTIN")]
        public string taxPayerTIN { get; set; }
        [DisplayName("DeviceId")]
        public string DeviceId { get; set; }
        [DisplayName("Allow Negative")]
        public bool AllowNegative1 { get; set; }
        [DisplayName("Show Qty")]
        public bool ShowQuantity { get; set; }
        [DisplayName("Logo")]
        public string baseLogo { get; set; }
    }
}

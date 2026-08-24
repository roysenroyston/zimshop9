using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class JobCard
    {

        [DisplayName(" ID")]
        public int Id { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [Required]
        [DisplayName("Order Number")]
        public int OrderNumber { get; set; }

        [Required]
        [DisplayName("Job Number")]
        public int JobNo { get; set; }

        [DisplayName("Sandries/Transport")]
        public Decimal sandries { get; set; }

        [DisplayName("Total Before VAT")]
        public Decimal totalbfvat { get; set; }

        [DisplayName("VAT")]
        public Decimal VAT { get; set; }

        [DisplayName("Total Amount With VAT")]
        public Nullable<Decimal> TotalAmountWithTax { get; set; }

        [DisplayName("Customer Name")]
        public int? customername { get; set; }
        public virtual User customername_customernameId { get; set; }

        [DisplayName("Customer Address")]
        public string address { get; set; }

        [DisplayName("Date Received")]
        public Nullable<DateTime> purchasedate { get; set; }

        [DisplayName("JobCard Invoiced")]
        public bool completed { get; set; }

        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }

      

        public IEnumerable<JobCardMaterials> JobCardMaterials { get; set; }
        public IEnumerable<JobCardServices> JobCardServices { get; set; }
    }
}
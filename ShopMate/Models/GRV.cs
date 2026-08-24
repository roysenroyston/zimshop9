using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ShopMate.Models
{
    [TrackChanges]
    public class GRV
    {
        [DisplayName(" ID")]
        public int Id { get; set; }
        //[Required]
        [DisplayName("Supplier")]
        public string supplier { get; set; }

        [DisplayName("Quantity")]
        public Decimal Quantity { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }
        [Required]
        [DisplayName("Recieved By")]
        public string receivedby { get; set; }
        //[Required]
        [DisplayName("Order Number")]
        public int OrderNumber { get; set; }

        
        [DisplayName("Unit Price")]
        public Nullable<Decimal> UnitPrice { get; set; }

        [DisplayName("Total Price")]
        public Decimal TotalPrice { get; set; }
        [DisplayName("approved")]
        public bool approved { get; set; }
        [DisplayName("Date Received")]       
        public Nullable<DateTime> purchasedate { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int Warehouse { get; set; }

        [DisplayName("ValidUntil")]
        public Nullable<DateTime> ValidUntil { get; set; }
        public IEnumerable<GRVMaterials> GRVMaterials { get; set; }

    }
}
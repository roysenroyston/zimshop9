using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class OrderNumber
    {
        public int OrderNumberId { get; set; }
        public int SupplierOrderNumber { get; set; }
        public int CustomerOrderNumber { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.ModelDto
{
    public class StoresDto
    {
        public int storeId { get; set; }
        public List<StoreMaterialDto> material { get; set; }
        public decimal Totalprice { get; set; }
        public Nullable<DateTime> purchasedate { get; set; }
        public string receivedby { get; set; }
        public string companayname { get; set; }
        public int strnum { get; set; }

    }
    public class StoreMaterialDto
    {
        public string goods { get; set; }
        public string name { get; set; }
        public decimal Quantity { get; set; }
        public decimal price { get; set; }
        public decimal total { get; set; }
    }
}
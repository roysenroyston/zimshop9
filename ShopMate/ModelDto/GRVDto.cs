using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.ModelDto
{
    public class GRVDto
    {
        public int Id { get; set; }
        public string supplier { get; set; }
        public string receivedby { get; set; }
        public int OrderNumber { get; set; }
        public Nullable<DateTime> purchasedate { get; set; }
        public List<GRVMaterialsDto> GRVMaterials { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContact { get; set; }
        public string SupplierInfo { get; set; }
        public string companayname { get; set; }
        public List<GRVMaterialsDto> items { get; set; }
        public string Warehouse { get; set; }
    }

    public class GRVMaterialsDto
    {
        public int? ProductId { get; set; }
        public int Id { get; set; }
        public Decimal Quantity { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public Decimal UnitPrice { get; set; }
        public Decimal TotalPrice { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.ModelDto
{
    public class DispatchDto
    {

        public int Id { get; set; }
        public List<DispatchMaterialsDto> items { get; set; }
        public int? InvoiceNo { get; set; }
        public string CustomerName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContact { get; set; }
        public string CompanyName { get; set; }
        public Nullable<DateTime> ddate { get; set; }
        public string ToInfo { get; set; }
        public string Logo { get; set; }
        public int InvoiceId { get; set; }
        public string DispatchedTo { get; set; }

    }
    public class DispatchMaterialsDto
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public string Description { get; set; }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.ModelDto
{
    public class QuotationDto
    {
        public int Id { get; set; }
        public decimal SubTotal { get; set; }
        public decimal VAT { get; set; }
        public decimal Total { get; set; }
        public string AddedBy { get; set; }
        public DateTime IssueDate { get; set; }
        public Nullable<DateTime> ValidUntil { get; set; }
        public string ModifiedBy { get; set; }
        public List<QuotationItemsDto> items { get; set; }
        public string Type { get; set; }
        public string CurrencySymbol { get; set; }
        public string CompanyName { get; set; }
        public string Logo { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContact { get; set; }
        public string customer { get; set; }
        public string ToInfo { get; set; }
        public string TaxInfo { get; set; }
        public string QuotationFooterText { get; set; }
    }
}

public class QuotationItemsDto
{
    public int Id { get; set; }
    public decimal Quantity { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
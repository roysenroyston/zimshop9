using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ShopMate.ModelDto
{
    public class JobCardDto
    {
        public int jobcardId { get; set; }
        public List<servicesDto> services { get; set; }
        public decimal SubTotal { get; set; }
        public string TaxInfo { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal sandries { get; set; }
        public List<JobcardMaterialsDto> materialsDtos { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int OrderNumber { get; set; }
        public bool completed { get; set; }
        public int JobNo { get; set; }
        public Nullable<DateTime> purchasedate { get; set; }
        public string ToInfo { get; set; }
        public string InvoiceFooterText { get; set; }

        public Nullable<Decimal> totalbfvat { get; set; }


        public Nullable<Decimal> VAT { get; set; }

        public string companayname { get; set; }
        public Nullable<Decimal> TotalAmountWithTax { get; set; }


        public string customername { get; set; }


        public string address { get; set; }

        public string CurrencySymbol { get; set; }
        public string CompanyName { get; set; }
        public string Logo { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContact { get; set; }
    }


    public class JobcardMaterialsDto
    {
        public string material { get; set; }
        public decimal Quantity { get; set; }
        public decimal price { get; set; }
    }


}


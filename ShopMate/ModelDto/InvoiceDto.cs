using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.ModelDto
{
    public class InvoiceDto
    {
        public int? InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceFooterText { get; set; }
        public string CurrencySymbol { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public string baseLogo { get; set; }
        public string Logo { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContact { get; set; }
        public string tinNo { get; set; }
        public string BP { get; set; }
        public string CompanyvatNo { get; set; }
        public int ProjectNumber { get; set; }


        public decimal total { get; set; }

        public decimal? payment { get; set; }

        public decimal? balance { get; set; }


        public decimal vat { get; set; }
        public string vatNo { get; set; }
        public string orderNo { get; set; }
        public string PaymentTerms { get; set; }
        public decimal? subtotal { get; set; }

        public DateTime? Duedate { get; set; }

        public string CustomerVatReg { get; set; }
        public string ToName { get; set; }
        public string Cstomer { get; set; }
        public string ToInfo { get; set; }
        public List<InvoiceItemsDto> invoiceItem { get; set; }
        public List<servicesDto> services { get; set; }
        public decimal? SubTotal { get; set; }
        public string TaxInfo { get; set; }
        public decimal? Tax { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? Change { get; set; }
        public decimal? totalDiscount { get; set; }

        public string Type { get; set; }
        public string rep { get; set; }


        public string deviceSerialNo { get; set; }
        public string fiscalDayNumber { get; set; }
        public string deviceID { get; set; }
        public int receiptId { get; set; }

        public string verificationCode { get; set; }
        public string Zimraemail { get; set; }
        public string email { get; set; }
        public string myVerificationCode { get; set; }
        public string qrCode { get; set; }
    }

    public class InvoiceItemsDto
    {
        public decimal Quantity { get; set; }
        public string ProcuctName { get; set; }
        public decimal Price { get; set; }
        public string TaxInfo { get; set; }
        public decimal Tax { get; set; }
        public decimal SubTotal { get; set; }
    }
    public class servicesDto
    {

        public string machineused { get; set; }
        public string artisan { get; set; }
        public string hours { get; set; }
        public decimal rate { get; set; }
        public decimal discount { get; set; }
        public decimal total { get; set; }
        public string productn { get; set; }
        public string description { get; set; }
        public decimal quantity { get; set; }
        public string vat { get; set; }
        public string Code { get; set; }
        public decimal rates { get; set; }
    }
}
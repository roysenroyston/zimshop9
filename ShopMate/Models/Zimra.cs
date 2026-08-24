using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShopMate.Models
{
    public class Zimra
    {
        public class receiptLines
        {
            public string receiptLineType { get; set; }
            public int receiptLineNo { get; set; }
            public string receiptLineHSCode { get; set; }
            public string receiptLineName { get; set; }
            public decimal receiptLinePrice { get; set; }
            public decimal receiptLineQuantity { get; set; }
            public decimal receiptLineTotal { get; set; }
            public string taxCode { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public double? taxPercent { get; set; }
            public int taxID { get; set; }
        }

        public class receiptTaxs
        {
            public string taxCode { get; set; }
            public double taxPercent { get; set; }
            public int taxID { get; set; }
            public decimal taxAmount { get; set; }
            public decimal? salesAmountWithTax { get; set; }
        }

        public class receiptPayments
        {
            public string moneyTypeCode { get; set; }
            public decimal? paymentAmount { get; set; }
        }

        public class CreditDebitNote
        {
            public int receiptID { get; set; }
        }

        public class receipt
        {
            public string receiptType { get; set; }
            public string receiptCurrency { get; set; }
            public int receiptCounter { get; set; }
            public int receiptGlobalNo { get; set; }
            public string invoiceNo { get; set; }
            public object buyerData { get; set; }
            public object receiptNotes { get; set; }
            public DateTime? receiptDate { get; set; }
            public CreditDebitNote creditDebitNote { get; set; }

            //public object creditDebitNote { get; set; }
            //  public object receiptID { get; set; }
            public bool receiptLinesTaxInclusive { get; set; }

            public List<receiptLines> receiptLines { get; set; }
            public List<receiptTaxs> receiptTaxes { get; set; }
            public List<receiptPayments> receiptPayments { get; set; }
            public decimal receiptTotal { get; set; }
            public string receiptPrintForm { get; set; }
        }
        public class BuyerData
        {
            public string BuyerRegisterName { get; set; }
            public string BuyerTradeName { get; set; }
            public BuyerContacts BuyerContacts { get; set; }
            public string BuyerTIN { get; set; }
            public string VATNumber { get; set; }
            public BuyerAddress BuyerAddress { get; set; }
        }

        public class BuyerContacts
        {
            public string PhoneNo { get; set; }
            public string Email { get; set; }
        }

        public class BuyerAddress
        {
            public string Province { get; set; }
            public string Street { get; set; }
            public string HouseNo { get; set; }
            public string City { get; set; }
        }
    }
}
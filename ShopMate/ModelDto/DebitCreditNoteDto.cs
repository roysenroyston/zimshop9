using System;
using System.Collections.Generic;

namespace ShopMate.ModelDto
{
    public class DebitCreditNoteDto
    {

        public int Id { get; set; }
        public bool IsFiscilazed { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> vat { get; set; }
        public Nullable<decimal> subtotal { get; set; }
        public Nullable<DateTime> Duedate { get; set; }
        public int WarehouseId { get; set; }
        public string Remarks { get; set; }
        public int RecieptId { get; set; }
        public string receiptNo { get; set; }
        public string ReceiptType { get; set; }
        public Nullable<int> AddedBy { get; set; }
        public List<DebitCreditNoteItemsDto> items { get; set; }

        public string customerAddress { get; set; }
        public string customerTin { get; set; }
        public string customerVat { get; set; }
        public string customer { get; set; }
        public string customerPhone { get; set; }
        public string customerEmail { get; set; }
      
       
     
      
        public string ToInfo { get; set; }
        public string TaxInfo { get; set; }

        public string CompanyName { get; set; }
        public string Logo { get; set; }
        public string baseLogo { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyContact { get; set; }

        public string QuotationFooterText { get; set; }

        public string CompanyVat { get; set; }

        public string tinNo { get; set; }
        public string email { get; set; }

        public string qrCode { get; set; }
        public string VerificationCode { get; set; } = string.Empty;
        public string qrUrl { get; set; }
        public string deviceSerialNo { get; set; }
        public string fiscalDayNumber { get; set; }
        public string deviceID { get; set; }



    }
    public class DebitCreditNoteItemsDto
    {
        public int Id { get; set; }
        public string receiptLineType { get; set; }
        public int receiptLineNo { get; set; }
        public string receiptLineHSCode { get; set; }
        public string receiptLineName { get; set; }
        public string vat { get; set; }
        public decimal receiptLinePrice { get; set; }
        public int receiptLineQuantity { get; set; }

        public decimal receiptLineTotal { get; set; }
        public int ReceiptNo { get; set; }
        public int receiptId { get; set; }
        public int debitCreditNoteId { get; set; }
        public bool isFiscal { get; set; }
        public string qrCode { get; set; }
        public string VerificationCode { get; set; }
        public string qrUrl { get; set; }
        public string deviceSerialNo { get; set; }
        public string fiscalDayNumber { get; set; }
        public string deviceID { get; set; }
    }

}
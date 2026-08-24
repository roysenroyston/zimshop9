using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class DebitCreditItem
    {
        public int Id { get; set; }
        public string receiptLineType { get; set; }
        public int receiptLineNo { get; set; }
        public string receiptLineHSCode { get; set; }
        public string receiptLineName { get; set; }
        public decimal receiptLinePrice { get; set; }
        public int receiptLineQuantity { get; set; }
        public decimal receiptLineTotal { get; set; }
        public string ReceiptNo { get; set; }
        public string HsnCode { get; set; }
        public int receiptId { get; set; }
        public int debitCreditNoteId { get; set; }
        public bool isFiscal { get; set; }
        public decimal lineVat { get; set; }
        public string qrCode { get; set; }
      //  public int receiptID { get; set; }
        public string VerificationCode { get; set; }
        public string qrUrl { get; set; }
        public string deviceSerialNo { get; set; }
        public string fiscalDayNumber { get; set; }
        public string deviceID { get; set; }
    }
}
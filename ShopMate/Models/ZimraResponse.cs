namespace ShopMate.Models
{
    public class DeviceBranchAddress
    {
        public string Province { get; set; }
        public string Street { get; set; }
        public string HouseNo { get; set; }
        public string City { get; set; }
    }

    public class DeviceBranchContacts
    {
        public string PhoneNo { get; set; }
        public string Email { get; set; }
    }

    public class myResponse
    {
        public string TaxPayerName { get; set; }
        public string TaxPayerTIN { get; set; }
        public string VatNumber { get; set; }
        public string DeviceBranchName { get; set; }
        public DeviceBranchAddress DeviceBranchAddress { get; set; }
        public DeviceBranchContacts DeviceBranchContacts { get; set; }
        public string TaxCode { get; set; }
        public string QrUrl { get; set; }
        public string DeviceSerialNo { get; set; }
        public string ReceiptCounter { get; set; }
        public string receiptGlobalNo { get; set; }
        public string FiscalDayNumber { get; set; }
        public int receiptID { get; set; }
        public string InvoiceNumber { get; set; }
        public string DeviceID { get; set; }
        public string Date { get; set; }
        public string TaxPercentage { get; set; }
        public string QrString { get; set; }
        public string VerificationCode { get; set; }
    }

    public class OpenDay
    {
        public int fiscalDayNo { get; set; }
        public string operationID { get; set; }

    }
    public class GetStatus
    {
        public string fiscalDayStatus { get; set; }
        public string operationID { get; set; }
        public int lastReceiptGlobalNo { get; set; }
        public int lastFiscalDayNo { get; set; }
        public string fiscalDayClosingErrorCode { get; set; }

    }



    public class errorResponse
    {
        public string error_type { get; set; }
        public string error_message { get; set; }
        public string traceback { get; set; }
        public string invalid_value { get; set; }


    }
}

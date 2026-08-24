using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.Models
{
    public class MySell
    {
        public string currency { get; set; }
        public string date { get; set; }
        public int id { get; set; }
        public int invoiceId { get; set; }
        public int online { get; set; }
        public List<SellProduct> products { get; set; }
        public string subtotal { get; set; }
        public double tax { get; set; }
        public string time { get; set; }
        public int userId { get; set; }
        public string paymentMethod { get; set; }
        public string customer { get; set; }
        public string discount { get; set; }
        public decimal rate { get; set; }


        public string qrUrl { get; set; }
        public string deviceSerialNo { get; set; }
     
        public string fiscalDayNumber { get; set; }
        public int zimraReceiptNo { get; set; }

        public string deviceID { get; set; }
 

        public string qrcode { get; set; }
        public string verificationCode { get; set; }
    }
    public class SellProduct
    {
        public string barcode { get; set; }
        public int id { get; set; }
        public string image { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }

        public decimal priceRtgs { get; set; }
        public int prodId { get; set; }
        public decimal quantity { get; set; }
        public double tax { get; set; }

       // public decimal UnitPrice { get; set; }
    }

    public class Dsales
    {

        public List<SalesItem> products { get; set; }  // List of items in the sale
        public decimal Subtotal { get; set; }  // Subtotal amount for the sale
        public decimal TaxAmount { get; set; }  // Total tax amount for the sale
        public decimal TotalAmount { get; set; }  // Total amount after tax
        public decimal PaidAmount { get; set; }  // Amount paid by the customer
        public decimal Change { get; set; }  // Change to be returned to the customer
        public DateTime Date { get; set; }  // Date of the sale
        public string UserId { get; set; }  // ID of the user performing the sale
        public decimal Rate { get; set; }  // Currency exchange rate, if applicable
        public string PaymentMethod { get; set; }  // Method of payment (e.g., Credit, Cash, etc.)
        public int Online { get; set; }  // Flag for online payment (1 for online, 0 for offline)
        public int InvoiceId { get; set; }  // Invoice ID associated with the sale
        public string Discount { get; set; }  // Any applicable discount in the sale
        public string Currency { get; set; }  // Currency of the transaction (e.g., USD)
        public string Customer { get; set; }  // Customer information for the sale

        // Additional properties from JSON
        public string date { get; set; }  // Date string in dd/MM/yyyy format
        public string time { get; set; }  // Time string in HH:mm:ss format
        public decimal tax { get; set; }  // Tax amount (maps to TaxAmount)
        public long id { get; set; }  // Sale ID


        public string QrUrl { get; set; }
        public string DeviceSerialNo { get; set; }

        public string FiscalDayNumber { get; set; }
        public int receiptID { get; set; }

        public string DeviceID { get; set; }


        public string QrString { get; set; }
        public string VerificationCode { get; set; }
    }
    public class SalesItem
    {
        public string name { get; set; }  // Name of the product sold
        public decimal price { get; set; }  // Price of the product at the time of sale
        public decimal tax { get; set; }  // Tax applicable on the product
        public decimal quantity { get; set; }  // Quantity of the product sold
        public decimal total { get; set; }  // Total amount for this particular product (SalePrice * Quantity)

        public string barcode { get; set; }  // Barcode of the product
        public int prodId { get; set; }  // Product ID for identification in the system
    }
}
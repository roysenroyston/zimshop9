using ShopMate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopMate.ModelDto
{
    public class SaleDto
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReturnedQuantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal RtgsPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal? WithTaxAmount { get; set; }
        public DateTime Dated { get; set; }
        public string companayname { get; set; }
        public bool isFormalSale { get; set; }
        public decimal CustomerUserId { get; set; }
        public string CustomerName { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDiscounted { get; set; }
        public decimal TaxAmount { get; set; }
        public string RecieptNumber { get; set; }
        public decimal TotalRtgs { get; set; }
        public int? userid { get; set; }



    }
    public class NProfitDto
    {
        public decimal Expense { get; set; }
        public decimal Sale { get; set; }
        public decimal PurchaseReturn { get; set; }
        public decimal Purchase { get; set; }
        public decimal SaleReturn { get; set; }
        public string companayname { get; set; }
        public decimal Balance { get; set; }
        public decimal VCost { get; set; }
    }

    public class ProductHistDto
    {
        public int? ProductId { get; set; }
        // public string ProductId { get; set; }
        public string ProductName { get; set; }
        public Decimal Quantity { get; set; }
        public Decimal ReturnedQuantity { get; set; }
        public Decimal RemainingQuantity { get; set; }
        public Decimal UnitPrice { get; set; }
        public Decimal SalePrice { get; set; }
        public Decimal PurchasePrice { get; set; }
        public Decimal? TotalSaleAmount { get; set; }
        public Decimal? TotalPurchaseAmount { get; set; }
        public Nullable<Decimal> TotalAmountWithTax { get; set; }
        public Nullable<DateTime> DateAdded { get; set; }
        public string AddedBy { get; set; }
        public string WarehouseId { get; set; }
        public int ReceiptNo { get; set; }
        public String InventoryTypeId { get; set; }



    }
    public class ManufacturingDto
    {


        public Decimal unitprice { get; set; }

        public Decimal Total { get; set; }

        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public DateTime Dated { get; set; }
        public string companayname { get; set; }
        public DateTime datedadded { get; set; }
        public int? warehouseid { get; set; }


    }
    public class RawMaterialDto
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal Amount { get; set; }
        public DateTime Dated { get; set; }
        public string companayname { get; set; }
        public int warehouseid { get; set; }



    }


    public class WarehouseStockAmountDto
    {
        public int? productId { get; set; }
        public string productName { get; set; }
        public decimal actualQuantity { get; set; }
        public decimal counted { get; set; }
        public decimal actualValue { get; set; }
        public decimal countedValue { get; set; }
        public decimal variance { get; set; }
        public decimal varianceValue { get; set; }
        public int value { get; set; }
        public int? stocktakeId { get; set; }
    }
    public class SupplierDto
    {
        public string Customername { get; set; }
        public string address { get; set; }
        public decimal Quantity { get; set; }
        public string contact { get; set; }
        public decimal saleprice { get; set; }
        public string ProductName { get; set; }
        public decimal? totalamount { get; set; }
        public DateTime dated { get; set; }
        public decimal? amountpaid { get; set; }
        public decimal? amountdue { get; set; }
        public int InvTypeId { get; set; }


    }
    public class CustomerDto
    {
        public string Customername { get; set; }
        public string address { get; set; }
        public decimal Quantity { get; set; }
        public string contact { get; set; }
        public decimal saleprice { get; set; }
        public string ProductName { get; set; }
        public decimal? totalamount { get; set; }
        public DateTime dated { get; set; }
        public decimal? amountpaid { get; set; }
        public decimal? amountdue { get; set; }


    }
    public class CashupDto
    {
        public string TilloperatorName { get; set; }
        public int Tilloperator { get; set; }
        public decimal totalsalesfortheday { get; set; }
        public decimal totalcash { get; set; }
        public decimal cashs { get; set; }
        public decimal ecocashs { get; set; }
        public decimal swipes { get; set; }
        public decimal Totalswipe { get; set; }
        public decimal totalecocash { get; set; }
        public decimal totalChange { get; set; }
        public decimal accountsales { get; set; }
        public decimal accountpayments{ get; set; }
        public DateTime? Dated { get; set; }
        public string companayname { get; set; }

    }
    public class GrvDto
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal TotalPuchaseAmount { get; set; }
        public string supplierId { get; set; }
        public DateTime Dated { get; set; }
        public string companayname { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Amount { get; set; }
        public string Supplier { get; set; }

    }
    public class PurchaseDto
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal TotalPuchaseAmount { get; set; }
        public string supplierId { get; set; }
        public DateTime Dated { get; set; }
        public string companayname { get; set; }
        public string AddedBy { get; set; }
        public string InvoiceNumber { get; set; }



    }

    public class StockAdjustmentDto
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public string Description { get; set; }
        public string InventoryType { get; set; }
        public string WareHouse { get; set; }
        public string AddedBy { get; set; }
        public DateTime? DateAdded { get; set; }
    }
    public class ShrinkageDto
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Amount { get; set; }
        public decimal WithTaxAmount { get; set; }
        public DateTime Dated { get; set; }
        public string companayname { get; set; }
        public string effect { get; set; }
        public string Description { get; set; }
    }
    public class StockAlertDto
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; } 
        public decimal StockAlert { get; set; }
        public string companayname { get; set; }
    }
    public class ExpiryAlertDto
    {
        public string ProductName { get; set; }
        public decimal PeriodToExpiry { get; set; }
        public string BatchNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal ExpiryAlert { get; set; }
    }

    public class StockAmountDto
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal stocktake { get; set; }
        public decimal purchaseprice { get; set; }
        public decimal saleprice { get; set; }
        public decimal cost { get; set; }
        public decimal Amount { get; set; }
        public int category { get; set; }
        public string companayname { get; set; }
    }

    public class ProfitDto
    {
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ProfitAmountWithTax { get; set; }
        public string companayname { get; set; }
    }

    public class ExpenseDto
    {
        //public string Remarks { get; set; }
        //public decimal Amount { get; set; }
        //public DateTime? Dated { get; set; }
        //public string companayname { get; set; }
        //public string AddedBy { get; set; }

        public int? expense { get; set; }
        public string Remarks { get; set; }
        public string Vendorname { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Dated { get; set; }
        public string companayname { get; set; }
        public string InvoiceNumber { get; set; }
        public string VatNumber { get; set; }
        public Decimal TaxAmount { get; set; }
        public string invoicedate { get; set; }
        public decimal subtotal { get; set; }
        public string ExpenseName { get; set; }
        public string VendorName { get; set; }
    }
    public class DueDto
    {
        public string Remarks { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Dated { get; set; }
        public bool IsReturn { get; set; }
        public string companayname { get; set; }
    }

    public class LadgerDto
    {
        public string Remarks { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Dated { get; set; }
        public string companayname { get; set; }

    }

    public class DayEndDto
    {
        public decimal Sale { get; set; }
        public decimal PurchaseReturn { get; set; }
        public decimal DueReturn { get; set; }

        public decimal TotalPlus { get; set; }

        public decimal Expense { get; set; }
        public decimal DueGiven { get; set; }
        public decimal SaleReturn { get; set; }
        public decimal Purchase { get; set; }

        public decimal TotalMinus { get; set; }

        public decimal Profit { get; set; }
        public decimal ProfitWithTax { get; set; }
        public decimal paymentmode { get; set; }
        public decimal rtgs { get; set; }
        public decimal swipeold { get; set; }
        public decimal ecocashold { get; set; }
        public decimal cashold { get; set; }
        public decimal ecocash { get; set; }
        public decimal Change { get; set; }
        public decimal telecash { get; set; }
        public decimal Fbc { get; set; }
        public decimal Acl { get; set; }
        public decimal Zipit { get; set; }
        public decimal onemoney { get; set; }
        public decimal? cashusd { get; set; }
        public decimal fca { get; set; }
        public decimal? accountsales { get; set; }
        public decimal nostro { get; set; }
        public decimal accountpayments { get; set; }
        public decimal stock { get; set; }
        public decimal stockplus { get; set; }
        public string companayname { get; set; }
        public decimal tSale { get; set; }
        public decimal? cashdeclared { get; set; }
        public decimal? ecocashdeclared { get; set; }
        public decimal? Fbcdeclared { get; set; }
        public decimal? Acldeclared { get; set; }
        public decimal? Zipitdeclared { set; get; }
        public decimal? telecashdeclared { get; set; }
        public decimal? onemoneydeclared { get; set; }
        public decimal? cashusddeclared { get; set; }
        public decimal? swipedeclared { get; set; }
        public decimal? outagecash { get; set; }
        public decimal? outagefbc { get; set; }
        public decimal? outageacl { get; set; }
        public decimal? outagezipit { get; set; }
        public decimal? outageecocash { get; set; }
        public decimal? outagetelecash { get; set; }
        public decimal? outageonemoney { get; set; }
        public decimal? outagecashusd { get; set; }
        public decimal? outageChange { get; set; }
        public decimal? outageswipe { get; set; }
        public decimal? outagetotal { get; set; }
        public decimal? outagecasho { get; set; }
        public decimal? outageecocasho { get; set; }
        public decimal? outageswipeo { get; set; }
        public decimal? outagetotalo { get; set; }
        public decimal? totaldeclared { get; set; }
        public decimal? accumulatedchange { get; set; }
    }
    public class VATReport
    {
        public decimal Sale { get; set; }
        public decimal PurchaseReturn { get; set; }
        public decimal salesexcludevat { get; set; }

        public decimal taxablesales { get; set; }

        public decimal taxonsales { get; set; }
        public decimal nontaxablesales { get; set; }
        public decimal? SaleReturn { get; set; }
        public decimal? totalPurchase { get; set; }

        public decimal Totapurchaseexcludingvat { get; set; }

        public decimal taxablepurchases { get; set; }
        public decimal nontaxablepurchase { get; set; }
        public decimal taxonpurchases { get; set; }
        public decimal nettax { get; set; }
       public decimal discountsonsales { get; set; }
        public decimal purchasesdiscount { get; set; }

        public decimal SaleZwg { get; set; }
        public decimal PurchaseReturnZwg { get; set; }
        public decimal salesexcludevatZwg { get; set; }

        public decimal taxablesalesZwg { get; set; }

        public decimal taxonsalesZwg { get; set; }
        public decimal nontaxablesalesZwg { get; set; }
        public decimal? SaleReturnZwg { get; set; }
        public decimal? totalPurchaseZwg { get; set; }

        public decimal TotapurchaseexcludingvatZwg { get; set; }

        public decimal taxablepurchasesZwg { get; set; }
        public decimal nontaxablepurchaseZwg { get; set; }
        public decimal taxonpurchasesZwg { get; set; }
        public decimal nettaxZwg { get; set; }
        public decimal discountsonsalesZwg { get; set; }
        public decimal purchasesdiscountZwg { get; set; }
        public DateTime? datefrom { get; set; }
        public DateTime? dateto { get; set; }
        public string companayname { get; set; }

    }

    public class WarehouseStockDto
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal Quantity { get; set; }
        public string ProductType { get; set; }
        public decimal StockValue { get; set; }
        public decimal Price { get; set; }
    }
}
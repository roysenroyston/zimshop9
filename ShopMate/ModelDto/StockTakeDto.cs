using System;
using System.Collections.Generic;

namespace ShopMate.ModelDto
{
    public class StockTakeDto
    {
        public int Id { get; set; }
        public int addedby { get; set; }
        public DateTime DateAdded { get; set; }
        public int warehouseId { get; set; }
        public List<StockTakeDetailsDto> StockTakeDetails { get; set; }
        public List<StockTakeDetailsDto> items { get; set; }
        public string companayname { get; set; }
    }

    public class StockTakeDetailsDto
    {
        public int? StockTakeId { get; set; }
        public string WarehouseId { get; set; }
        public decimal variancevalue { get; set; }
        public decimal variance { get; set; }
        public decimal countedvalue { get; set; }
        public decimal actualvalue { get; set; }
        public decimal counted { get; set; }
        public decimal actualquantity { get; set; }
        public string ProductId { get; set; }
    }
    public class ProductHistoryDto
    {
        public int Id { get; set; }

        public string ProductName { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal RemainingQty { get; set; }
        public Boolean IsActive { get; set; }
        public string Warehouse { get; set; }
        public List<ProductHistoryitemsDto> items { get; set; }
    }



    public class ProductHistoryitemsDto
    {

        public int Id { get; set; }


        public int? ProductId { get; set; }
        // public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string NewProductName { get; set; }
        public Decimal Quantity { get; set; }
        public Decimal ReturnedQuantity { get; set; }
        public Decimal UnitPrice { get; set; }
        public Decimal SalePrice { get; set; }
        public Decimal RemainingQty { get; set; }
        public Decimal? TotalSaleAmount { get; set; }
        public Decimal? TotalPurchaseAmount { get; set; }
        public Nullable<Decimal> TotalAmountWithTax { get; set; }
        public DateTime DateAdded { get; set; }
        public Nullable<int> AddedBy { get; set; }
        public string WarehouseId { get; set; }
        public int ReceiptNo { get; set; }
        public String InventoryTypeId { get; set; }
    }

}
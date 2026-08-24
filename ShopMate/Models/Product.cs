using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
namespace ShopMate.Models
{
    [TrackChanges]
    public class Product
    {
        [DisplayName("S.No")]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Product Category")]
        public int? ProductCategoryId { get; set; }

        [SkipTracking]
        [StringLength(100)]
        [DisplayName("Bar Code")]
        public string BarCode { get; set; }
        [StringLength(100)]
        [DisplayName("HSN Code")]
        public string HSNCode { get; set; }

        [DisplayName("Purchase Price")]
        public Decimal PurchasePrice { get; set; }
        [Required]
        [DisplayName("Sale Price USD")]
        public Decimal SalePrice { get; set; }
        [SkipTracking]
        [DisplayName("Sale Price RTGS")]//ngoni
        public Nullable<Decimal> RtgsPrice { get; set; }//ngoni
        [Required]
        [DisplayName("Weight")]
        public Decimal Weight { get; set; }
        [SkipTracking]
        [DisplayName("Product Image")]
        public string ProductImage { get; set; }
        [SkipTracking]
        [DisplayName("Product Description")]
        public string ProductDescription { get; set; }
        [DisplayName("Added By")]
        public Nullable<int> AddedBy { get; set; }
        [DisplayName("Date Added")]
        public Nullable<DateTime> DateAdded { get; set; }
        [DisplayName("Modified By")]
        public Nullable<int> ModifiedBy { get; set; }
        [DisplayName("Date Modied")]
        public Nullable<DateTime> DateModied { get; set; }
        [SkipTracking]
        [Required]
        [DisplayName("Is Active")]
        public bool IsActive { get; set; }
        [SkipTracking]
        [Required]
        [DisplayName("Stock Reorder Level")]
        public int StockAlert { get; set; }
        //[Required]
        //[DisplayName("Expiry Alert")]
        //public int ExpiryAlert { get; set; }        
        [Required]
        [SkipTracking]
        [DisplayName("Tax")]
        public Nullable<int> TaxId { get; set; }
        [Required]
        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }
        [SkipTracking]
        [DisplayName("Discount")]
        public Nullable<Decimal> Discount { get; set; }

        [DisplayName("Remaining Quantity")]
        [DefaultValue(0.00)]
        public Decimal RemainingQuantity { get; set; }

        [DisplayName("Remaining Amount")]
        [DefaultValue(0.00)]
        public Decimal RemainingAmount { get; set; }
        //[SkipTracking]
        //[StringLength(100)]
        //[DisplayName("HSN")]
        //public string HSN { get; set; }
        public int? ProductCaseId { get; set; }
        public int? NumOfSinglesInCase { get; set; }
        [DisplayName("Product Type")]
        public string productType { get; set; }
        //[SkipTracking]

        //[DisplayName("Events Usd Price")]
        //public decimal eventsUsdPrice { get; set; }
        //[SkipTracking]
        //[DisplayName("Events Rtgs Price")]
        //public decimal eventsRtgsPrice { get; set; }

        public virtual ICollection<ProductStock> ProductStock_ProductIds { get; set; }
        public virtual ICollection<Sale> Sale_ProductIds { get; set; }
        public virtual ICollection<InvoiceItems> InvoiceItems_ProductIds { get; set; }
        public virtual ICollection<Purchase> Purchase_ProductIds { get; set; }

    }
    public static class Extensions
    {
        public static DataTable ToDataTable<T>(this List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties  
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table   
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows  
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}

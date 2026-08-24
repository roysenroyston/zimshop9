using System.Data.Entity;
using TrackerEnabledDbContext;

namespace ShopMate.Models
{
    public class SIContext : TrackerContext//DbContext
    {
        public SIContext()
            : base("name=SIConnectionString")
        {
        }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<AuditLogger> AuditLoggers { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<MenuPermission> MenuPermissions { get; set; }
        public virtual DbSet<ProductCategory> ProductCategorys { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Tax> Taxs { get; set; }
        public virtual DbSet<ProductStock> ProductStock { get; set; }
        public virtual DbSet<PaymentMode> PaymentModes { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<InvoiceItems> InvoiceItemss { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<InvoiceFormat> InvoiceFormats { get; set; }
        public virtual DbSet<InventoryType> InventoryTypes { get; set; }
        public virtual DbSet<Purchase> Purchases { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<LedgerAccount> LedgerAccounts { get; set; }
        public virtual DbSet<Warehouse> Warehouses { get; set; }
        public virtual DbSet<DuePayment> DuePayments { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<DeclaredayEnd> DayEnds { get; set; }
        public virtual DbSet<Paymenttrack> Paymenttracks { get; set; }
        public virtual DbSet<GRVMaterials> GRVMaterials { get; set; }
        public virtual DbSet<Quotation> Quotations { get; set; }
        public virtual DbSet<QuotationItems> QuotationItems { get; set; }
        public virtual DbSet<DNoteMaterial> DNoteMaterials { get; set; }
        public virtual DbSet<InvoiceMaterials> InvoiceMaterial { get; set; }
        public virtual DbSet<JobCardServices> JobCardServices { get; set; }
        public virtual DbSet<JobCardMaterials> JobCardMaterials { get; set; }
        public virtual DbSet<Machine> Machines { get; set; }
        public virtual DbSet<DeliveryNote> DeliveryNotes { get; set; }
        public virtual DbSet<GRV> GRVs { get; set; }
        public virtual DbSet<JobCard> JobCards { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderMaterials> OrderMaterial { get; set; }
        public virtual DbSet<StockTake> StockTakes { get; set; }
        public virtual DbSet<StockTakeDetails> StockTakeDetail { get; set; }
        public virtual DbSet<Stores> Store { get; set; }
        public virtual DbSet<StoresMaterials> StoreMaterial { get; set; }
        public virtual DbSet<RawMaterials> RawMaterial { get; set; }
        public virtual DbSet<Manufacturing> Manufacturing { get; set; }
        public virtual DbSet<RawMaterialStock> RawMaterialStocks { get; set; }
        public virtual DbSet<Accountpayment> AccountPayments { get; set; }
        public virtual DbSet<Dispatch> Dispatches { get; set; }
        public virtual DbSet<DispatchMaterials> Dispatchmaterial { get; set; }
        public virtual DbSet<Rate> Rates { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<FinishedGoods> FinishedGoods { get; set; }
        public virtual DbSet<finishedItem> FinishedItems { get; set; }
        public virtual DbSet<SaleOrder> SaleOrders { get; set; }
        public virtual DbSet<InvoiceType> InvoiceTypes { get; set; }

        public virtual DbSet<SaleOrderItem> SaleOrderItems { get; set; }
        public virtual DbSet<InformalInvoice> InformalInvoices { get; set; }
        public virtual DbSet<InvoicePaymentMethod> InvoicePaymentMethods { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }
        public virtual DbSet<Bank> Banks { get; set; }
        public virtual DbSet<FinishedManufacturedItem> FinishedManufacturedItems { get; set; }
        public virtual DbSet<ProductBatch> ProductBatches { get; set; }
        public virtual DbSet<Van> Vans { get; set; }
        public virtual DbSet<VanSale> VanSales { get; set; }
        public virtual DbSet<VanSaleItem> VanSaleItems { get; set; }
        public virtual DbSet<WarehouseStock> WarehouseStocks { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<fiscalday> Fiscaldays { get; set; }
        //public virtual DbSet<VanSale> VanSales { get; set; }
        //public virtual DbSet<VanSaleItem> VanSaleItems { get; set; }
        public virtual DbSet<DebitCreditNote> DebitCreditNotes { get; set; }
        public virtual DbSet<DebitCreditItem> DebitCreditItems { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Configurations.Add(new ShopMate.Maping.RoleMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.UserMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.MenuMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.MenuPermissionMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.ProductCategoryMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.ProductMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.TaxMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.ProductStockMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.PaymentModeMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.SaleMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.InvoiceItemsMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.SettingMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.InvoiceFormatMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.InventoryTypeMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.PurchaseMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.TransactionMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.InvoiceMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.LedgerAccountMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.WarehouseMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.DuePaymentMap());
            modelBuilder.Configurations.Add(new ShopMate.Maping.ExpenseMap());




            base.OnModelCreating(modelBuilder);
        }
        //
    }
}


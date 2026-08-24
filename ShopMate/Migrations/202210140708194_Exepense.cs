namespace ShopMate.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Exepense : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Expense", "SubTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Expense", "InvoiceDate", c => c.String());
            AddColumn("dbo.Expense", "Vendorname", c => c.Int());
            DropColumn("dbo.Expense", "CurrencyId");
            DropColumn("dbo.Expense", "CurrencyAmount");
            DropColumn("dbo.Expense", "CurrencyTaxAmount");
        }

        public override void Down()
        {
            AddColumn("dbo.Expense", "CurrencyTaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Expense", "CurrencyAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Expense", "CurrencyId", c => c.Int(nullable: false));
            DropColumn("dbo.Expense", "Vendorname");
            DropColumn("dbo.Expense", "InvoiceDate");
            DropColumn("dbo.Expense", "SubTotal");
        }
    }
}

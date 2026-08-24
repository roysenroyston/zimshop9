namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Allinone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InformalInvoices", "Currencytotal", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.InformalInvoices", "Currencyvat", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.InformalInvoices", "Currencysubtotal", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InformalInvoices", "Currencysubtotal");
            DropColumn("dbo.InformalInvoices", "Currencyvat");
            DropColumn("dbo.InformalInvoices", "Currencytotal");
        }
    }
}

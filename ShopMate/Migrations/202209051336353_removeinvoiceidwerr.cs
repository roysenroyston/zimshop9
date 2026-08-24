namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeinvoiceidwerr : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ProductStock", "CGST");
            DropColumn("dbo.ProductStock", "CGST_Rate");
            DropColumn("dbo.ProductStock", "SGST");
            DropColumn("dbo.ProductStock", "SGST_Rate");
            DropColumn("dbo.ProductStock", "IGST");
            DropColumn("dbo.ProductStock", "IGST_Rate");
            DropColumn("dbo.ProductStock", "OtherTaxValue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductStock", "OtherTaxValue", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ProductStock", "IGST_Rate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ProductStock", "IGST", c => c.Int());
            AddColumn("dbo.ProductStock", "SGST_Rate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ProductStock", "SGST", c => c.Int());
            AddColumn("dbo.ProductStock", "CGST_Rate", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ProductStock", "CGST", c => c.Int());
        }
    }
}

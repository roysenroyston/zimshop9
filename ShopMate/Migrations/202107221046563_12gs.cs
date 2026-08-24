namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _12gs : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.ProductStock", "InvoiceNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductStock", "InvoiceNumber");
        }
    }
}

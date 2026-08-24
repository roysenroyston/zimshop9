namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addnegative : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceFormat", "AllowNegative1", c => c.Boolean(nullable: false));
            AddColumn("dbo.InvoiceFormat", "ShowQuantity", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InvoiceFormat", "ShowQuantity");
            DropColumn("dbo.InvoiceFormat", "AllowNegative1");
        }
    }
}

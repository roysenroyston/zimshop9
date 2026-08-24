namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class vn : DbMigration
    {
        public override void Up()
        {
            //DropColumn("dbo.Purchase", "VatNumber");
            //DropColumn("dbo.Purchase", "InvoiceNumber");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Purchase", "InvoiceNumber", c => c.String());
            AddColumn("dbo.Purchase", "VatNumber", c => c.String());
        }
    }
}

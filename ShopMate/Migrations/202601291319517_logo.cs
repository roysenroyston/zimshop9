namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class logo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceFormat", "baseLogo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InvoiceFormat", "baseLogo");
        }
    }
}

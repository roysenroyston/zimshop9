namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddiscounttosale : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.InvoiceItems", "discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AddColumn("dbo.Sale", "discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sale", "discount");
            DropColumn("dbo.InvoiceItems", "discount");
        }
    }
}

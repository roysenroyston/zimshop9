namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _90dn : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.Purchase", "InvoiceNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Purchase", "InvoiceNumber", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}

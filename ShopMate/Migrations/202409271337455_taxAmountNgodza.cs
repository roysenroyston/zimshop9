namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class taxAmountNgodza : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Purchase", "TaxAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Purchase", "TaxAmount");
        }
    }
}

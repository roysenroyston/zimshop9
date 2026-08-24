namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class events : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "productType", c => c.String());
            AddColumn("dbo.Product", "eventsUsdPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Product", "eventsRtgsPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Product", "eventsRtgsPrice");
            DropColumn("dbo.Product", "eventsUsdPrice");
            DropColumn("dbo.Product", "productType");
        }
    }
}

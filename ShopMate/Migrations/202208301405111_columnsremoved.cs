namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class columnsremoved : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Product", "HSN");
            DropColumn("dbo.Product", "eventsUsdPrice");
            DropColumn("dbo.Product", "eventsRtgsPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Product", "eventsRtgsPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Product", "eventsUsdPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Product", "HSN", c => c.String(maxLength: 100));
        }
    }
}

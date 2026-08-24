namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rtgs : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Sale", "TotalRtgsAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AddColumn("dbo.Sale", "RtgsPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AddColumn("dbo.Sale", "PaidRtgsAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sale", "PaidRtgsAmount");
            DropColumn("dbo.Sale", "RtgsPrice");
            DropColumn("dbo.Sale", "TotalRtgsAmount");
        }
    }
}

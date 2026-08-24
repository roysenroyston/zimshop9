namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddiscounttoproductstocklkaddback : DbMigration
    {
        public override void Up()
        {
            ////AlterColumn("dbo.ProductStock", "discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductStock", "discount", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}

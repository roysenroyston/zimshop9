namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddiscounttoproductstocklk : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.ProductStock", "Discount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ProductStock", "Discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}

namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class discounttodhj : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.ProductStock", "ddiscount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductStock", "ddiscount");
        }
    }
}

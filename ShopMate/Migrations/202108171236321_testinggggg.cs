namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testinggggg : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.VanSales", "IsReturned", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VanSales", "IsReturned");
        }
    }
}

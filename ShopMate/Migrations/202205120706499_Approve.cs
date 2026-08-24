namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Approve : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VanSales", "approved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VanSales", "approved");
        }
    }
}

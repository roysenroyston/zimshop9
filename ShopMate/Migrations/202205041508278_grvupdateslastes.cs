namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class grvupdateslastes : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.VanSales", "Route", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VanSales", "Route");
        }
    }
}

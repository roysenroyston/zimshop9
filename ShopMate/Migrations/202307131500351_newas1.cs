namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newas1 : DbMigration
    {
        public override void Up()
        {
          //  AddColumn("dbo.Rates", "WarehouseId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rates", "WarehouseId");
        }
    }
}

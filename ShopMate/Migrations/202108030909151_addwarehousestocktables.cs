namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addwarehousestocktables : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "dbo.WarehouseStocks",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            ProductId = c.Int(nullable: false),
            //            WarehouseId = c.Int(nullable: false),
            //            RemainingQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
            //        })
            //    .PrimaryKey(t => t.Id)
            //    .ForeignKey("dbo.Product", t => t.ProductId, cascadeDelete: true)
            //    .ForeignKey("dbo.Warehouse", t => t.WarehouseId, cascadeDelete: true)
            //    .Index(t => t.ProductId)
            //    .Index(t => t.WarehouseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WarehouseStocks", "WarehouseId", "dbo.Warehouse");
            DropForeignKey("dbo.WarehouseStocks", "ProductId", "dbo.Product");
            DropIndex("dbo.WarehouseStocks", new[] { "WarehouseId" });
            DropIndex("dbo.WarehouseStocks", new[] { "ProductId" });
            DropTable("dbo.WarehouseStocks");
        }
    }
}

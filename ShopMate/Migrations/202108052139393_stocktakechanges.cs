namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stocktakechanges : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.StockTakes", "ProductId", "dbo.Product");
            //DropForeignKey("dbo.StockTakes", "ProductCategoryId", "dbo.ProductCategory");
            //DropIndex("dbo.StockTakes", new[] { "ProductCategoryId" });
            //DropIndex("dbo.StockTakes", new[] { "ProductId" });
            //DropColumn("dbo.StockTakes", "ProductCategoryId");
            //DropColumn("dbo.StockTakes", "ProductId");
            //DropColumn("dbo.StockTakes", "actualquantity");
            //DropColumn("dbo.StockTakes", "counted");
            //DropColumn("dbo.StockTakes", "variancevalue");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StockTakes", "variancevalue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.StockTakes", "counted", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.StockTakes", "actualquantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.StockTakes", "ProductId", c => c.Int(nullable: false));
            AddColumn("dbo.StockTakes", "ProductCategoryId", c => c.Int());
            CreateIndex("dbo.StockTakes", "ProductId");
            CreateIndex("dbo.StockTakes", "ProductCategoryId");
            AddForeignKey("dbo.StockTakes", "ProductCategoryId", "dbo.ProductCategory", "Id");
            AddForeignKey("dbo.StockTakes", "ProductId", "dbo.Product", "Id", cascadeDelete: true);
        }
    }
}

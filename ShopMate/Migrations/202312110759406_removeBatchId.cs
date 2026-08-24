namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeBatchId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProductStock", "ProductBatchId", "dbo.ProductBatches");
            DropIndex("dbo.ProductStock", new[] { "ProductBatchId" });
            DropColumn("dbo.ProductStock", "ProductBatchId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ProductStock", "ProductBatchId", c => c.Int(nullable: false));
            CreateIndex("dbo.ProductStock", "ProductBatchId");
            AddForeignKey("dbo.ProductStock", "ProductBatchId", "dbo.ProductBatches", "Id", cascadeDelete: true);
        }
    }
}

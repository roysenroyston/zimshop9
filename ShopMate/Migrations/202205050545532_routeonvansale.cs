namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class routeonvansale : DbMigration
    {
        public override void Up()
        {
            //CreateIndex("dbo.VanSaleItems", "VanSaleId");
            //AddForeignKey("dbo.VanSaleItems", "VanSaleId", "dbo.VanSales", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VanSaleItems", "VanSaleId", "dbo.VanSales");
            DropIndex("dbo.VanSaleItems", new[] { "VanSaleId" });
        }
    }
}

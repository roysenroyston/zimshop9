namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class salesduplication : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Sale", new[] { "ProductId" });
            AlterColumn("dbo.Sale", "recieptNumber", c => c.Int(nullable: false));
            CreateIndex("dbo.Sale", new[] { "ProductId", "recieptNumber" }, unique: true, name: "IX_RecieptProduct");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Sale", "IX_RecieptProduct");
            AlterColumn("dbo.Sale", "recieptNumber", c => c.String());
            CreateIndex("dbo.Sale", "ProductId");
        }
    }
}

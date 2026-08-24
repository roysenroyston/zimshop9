namespace ShopMate.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class RemovePCategory : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Product", new[] { "ProductCategoryId" });
            AlterColumn("dbo.Product", "ProductCategoryId", c => c.Int());
            CreateIndex("dbo.Product", "ProductCategoryId");
        }

        public override void Down()
        {
            DropIndex("dbo.Product", new[] { "ProductCategoryId" });
            AlterColumn("dbo.Product", "ProductCategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Product", "ProductCategoryId");
        }
    }
}

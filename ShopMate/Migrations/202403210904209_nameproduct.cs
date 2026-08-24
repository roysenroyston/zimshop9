namespace ShopMate.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class nameproduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductStock", "ProductName", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.ProductStock", "ProductName");
        }
    }
}

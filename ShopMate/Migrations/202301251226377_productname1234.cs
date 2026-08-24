namespace ShopMate.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class productname1234 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockTakeDetails", "productName", c => c.String());
            AddColumn("dbo.StockTakeDetails", "DateAdded", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("dbo.StockTakeDetails", "DateAdded");
            DropColumn("dbo.StockTakeDetails", "productName");
        }
    }
}

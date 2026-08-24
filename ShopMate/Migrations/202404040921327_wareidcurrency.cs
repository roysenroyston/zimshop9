namespace ShopMate.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class wareidcurrency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Currencies", "WarehouseId", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Currencies", "WarehouseId");
        }
    }
}

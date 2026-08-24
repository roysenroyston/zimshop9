namespace ShopMate.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class printerMacaddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "printerMacAddress", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.User", "printerMacAddress");
        }
    }
}

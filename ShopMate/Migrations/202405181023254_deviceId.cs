namespace ShopMate.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class deviceId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceFormat", "DeviceId", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.InvoiceFormat", "DeviceId");
        }
    }
}

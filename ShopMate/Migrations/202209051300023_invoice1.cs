namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invoice1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Purchase", "invoiceid", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Purchase", "invoiceid");
        }
    }
}

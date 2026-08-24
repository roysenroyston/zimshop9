namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class invoice : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Sale", "recieptNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sale", "recieptNumber");
        }
    }
}

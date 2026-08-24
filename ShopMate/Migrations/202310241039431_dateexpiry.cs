namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateexpiry : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Warehouse", "ExpiryDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Warehouse", "ExpiryDate");
        }
    }
}

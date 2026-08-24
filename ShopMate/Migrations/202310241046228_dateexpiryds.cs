namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateexpiryds : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Warehouse", "DateCreated", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Warehouse", "DateCreated", c => c.DateTime(nullable: false));
        }
    }
}

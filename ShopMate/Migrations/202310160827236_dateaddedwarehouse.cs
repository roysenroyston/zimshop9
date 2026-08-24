namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateaddedwarehouse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Warehouse", "DateCreated", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Warehouse", "DateCreated");
        }
    }
}

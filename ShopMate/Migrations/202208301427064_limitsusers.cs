namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class limitsusers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Warehouse", "NumberOfUsers", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Warehouse", "NumberOfUsers");
        }
    }
}

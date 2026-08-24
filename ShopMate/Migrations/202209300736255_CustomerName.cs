namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sale", "CustomerName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sale", "CustomerName");
        }
    }
}

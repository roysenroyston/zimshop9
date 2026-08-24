namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class expiryremoved : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Product", "ExpiryAlert");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Product", "ExpiryAlert", c => c.Int(nullable: false));
        }
    }
}

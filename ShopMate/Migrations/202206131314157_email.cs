namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class email : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Email", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "Email");
        }
    }
}

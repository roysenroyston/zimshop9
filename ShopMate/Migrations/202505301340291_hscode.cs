namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hscode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "HSNCode", c => c.String(maxLength: 100));
            AddColumn("dbo.DebitCreditItems", "HsnCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DebitCreditItems", "HsnCode");
            DropColumn("dbo.Product", "HSNCode");
        }
    }
}

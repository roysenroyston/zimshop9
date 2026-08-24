namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paytrekfbc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Paymenttracks", "zipit", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paymenttracks", "fbc", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Paymenttracks", "acl", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Paymenttracks", "acl");
            DropColumn("dbo.Paymenttracks", "fbc");
            DropColumn("dbo.Paymenttracks", "zipit");
        }
    }
}

namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class grvupdateslast : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.GRVs", "Warehouse", c => c.Int(nullable: false));
            //AddColumn("dbo.GRVs", "ValidUntil", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.GRVs", "ValidUntil");
            DropColumn("dbo.GRVs", "Warehouse");
        }
    }
}

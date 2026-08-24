namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class grvupdates : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.GRVs", "supplier", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.GRVs", "supplier", c => c.String(nullable: false));
        }
    }
}

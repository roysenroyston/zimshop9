namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeproductstructure : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Product", "ProductCaseId", c => c.Int(nullable: false));
            //AddColumn("dbo.Product", "NumOfSinglesInCase", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Product", "NumOfSinglesInCase");
            DropColumn("dbo.Product", "ProductCaseId");
        }
    }
}

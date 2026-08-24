namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeproductstructureg : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Product", "ProductCaseId", c => c.Int());
            AlterColumn("dbo.Product", "NumOfSinglesInCase", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Product", "NumOfSinglesInCase", c => c.Int(nullable: false));
            AlterColumn("dbo.Product", "ProductCaseId", c => c.Int(nullable: false));
        }
    }
}

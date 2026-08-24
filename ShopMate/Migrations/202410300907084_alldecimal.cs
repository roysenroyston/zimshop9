namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alldecimal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InformalInvoices", "Change", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InformalInvoices", "Change");
        }
    }
}

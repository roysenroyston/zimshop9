namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productlinetax : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DebitCreditItems", "lineVat", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DebitCreditItems", "lineVat");
        }
    }
}

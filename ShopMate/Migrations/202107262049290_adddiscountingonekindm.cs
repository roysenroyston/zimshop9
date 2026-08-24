namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddiscountingonekindm : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Invoice", "totalDiscount", c => c.Decimal(precision: 18, scale: 2));
            //DropColumn("dbo.Invoice", "discount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoice", "discount", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.Invoice", "totalDiscount");
        }
    }
}

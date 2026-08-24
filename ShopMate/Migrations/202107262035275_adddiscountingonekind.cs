namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddiscountingonekind : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.InvoiceMaterials", "discount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InvoiceMaterials", "discount");
        }
    }
}

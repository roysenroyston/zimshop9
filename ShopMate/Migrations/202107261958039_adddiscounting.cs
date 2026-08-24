namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddiscounting : DbMigration
    {
        public override void Up()
        {
            //AddColumn("dbo.Invoice", "discount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Invoice", "discount");
        }
    }
}

namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payments_zipitacfbc : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeclaredayEnds", "Fbc", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.DeclaredayEnds", "Zipit", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.DeclaredayEnds", "Acl", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeclaredayEnds", "Acl");
            DropColumn("dbo.DeclaredayEnds", "Zipit");
            DropColumn("dbo.DeclaredayEnds", "Fbc");
        }
    }
}

namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _50bondnotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DeclaredayEnds", "fiftybond", c => c.Int());
            AddColumn("dbo.DeclaredayEnds", "hundredbond", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DeclaredayEnds", "hundredbond");
            DropColumn("dbo.DeclaredayEnds", "fiftybond");
        }
    }
}

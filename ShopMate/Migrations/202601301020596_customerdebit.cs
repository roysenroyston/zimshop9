namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customerdebit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DebitCreditNotes", "CustomerId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DebitCreditNotes", "CustomerId");
        }
    }
}

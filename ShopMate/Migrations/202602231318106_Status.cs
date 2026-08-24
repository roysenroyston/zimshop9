namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Status : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DebitCreditNotes", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DebitCreditNotes", "Status");
        }
    }
}

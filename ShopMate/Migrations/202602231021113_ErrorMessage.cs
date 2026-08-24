namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ErrorMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DebitCreditNotes", "ErrorMessage", c => c.String());
            AddColumn("dbo.InformalInvoices", "ErrorMessage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InformalInvoices", "ErrorMessage");
            DropColumn("dbo.DebitCreditNotes", "ErrorMessage");
        }
    }
}

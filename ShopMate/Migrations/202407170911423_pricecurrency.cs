namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pricecurrency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DebitCreditNotes", "ReceiptCurrency", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DebitCreditNotes", "ReceiptCurrency");
        }
    }
}

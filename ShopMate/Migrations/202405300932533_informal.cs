namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class informal : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.InformalInvoices", new[] { "InvoiceNo", "AddedBy" }, unique: true, name: "IX_RecieptProduct");
        }
        
        public override void Down()
        {
            DropIndex("dbo.InformalInvoices", "IX_RecieptProduct");
        }
    }
}

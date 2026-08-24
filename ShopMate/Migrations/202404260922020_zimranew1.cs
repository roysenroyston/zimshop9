namespace ShopMate.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class zimranew1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DebitCreditItems",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    receiptLineType = c.String(),
                    receiptLineNo = c.Int(nullable: false),
                    receiptLineHSCode = c.String(),
                    receiptLineName = c.String(),
                    receiptLinePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                    receiptLineQuantity = c.Int(nullable: false),
                    receiptLineTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    ReceiptNo = c.String(),
                    receiptId = c.Int(nullable: false),
                    debitCreditNoteId = c.Int(nullable: false),
                    isFiscal = c.Boolean(nullable: false),
                    qrCode = c.String(),
                    VerificationCode = c.String(),
                    qrUrl = c.String(),
                    deviceSerialNo = c.String(),
                    fiscalDayNumber = c.String(),
                    deviceID = c.String(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.DebitCreditNotes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    IsFiscilazed = c.Boolean(nullable: false),
                    InvoiceNo = c.String(),
                    total = c.Decimal(precision: 18, scale: 2),
                    vat = c.Decimal(precision: 18, scale: 2),
                    subtotal = c.Decimal(precision: 18, scale: 2),
                    Duedate = c.DateTime(),
                    WarehouseId = c.Int(nullable: false),
                    Remarks = c.String(),
                    RecieptId = c.Int(nullable: false),
                    receiptNo = c.String(),
                    ReceiptType = c.String(),
                    AddedBy = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Warehouse", t => t.WarehouseId, cascadeDelete: true)
                .Index(t => t.WarehouseId);

            AddColumn("dbo.Sale", "InvoiceId", c => c.Int());
            AddColumn("dbo.Sale", "zimraReceiptNo", c => c.Int(nullable: false));
            AddColumn("dbo.Sale", "qrUrl", c => c.String());
            AddColumn("dbo.Sale", "deviceSerialNo", c => c.String());
            AddColumn("dbo.Sale", "fiscalDayNumber", c => c.String());
            AddColumn("dbo.Sale", "deviceID", c => c.String());
            AddColumn("dbo.Sale", "isFiscalised", c => c.Boolean(nullable: false));
            AddColumn("dbo.Sale", "qrCode", c => c.String());
            AddColumn("dbo.Sale", "zimraRecieptNo", c => c.String());
            AddColumn("dbo.Sale", "VerificationCode", c => c.String());
            AddColumn("dbo.InvoiceFormat", "taxPayerTIN", c => c.String());
        }

        public override void Down()
        {
            DropForeignKey("dbo.DebitCreditNotes", "WarehouseId", "dbo.Warehouse");
            DropIndex("dbo.DebitCreditNotes", new[] { "WarehouseId" });
            DropColumn("dbo.InvoiceFormat", "taxPayerTIN");
            DropColumn("dbo.Sale", "VerificationCode");
            DropColumn("dbo.Sale", "zimraRecieptNo");
            DropColumn("dbo.Sale", "qrCode");
            DropColumn("dbo.Sale", "isFiscalised");
            DropColumn("dbo.Sale", "deviceID");
            DropColumn("dbo.Sale", "fiscalDayNumber");
            DropColumn("dbo.Sale", "deviceSerialNo");
            DropColumn("dbo.Sale", "qrUrl");
            DropColumn("dbo.Sale", "zimraReceiptNo");
            DropColumn("dbo.Sale", "InvoiceId");
            DropTable("dbo.DebitCreditNotes");
            DropTable("dbo.DebitCreditItems");
        }
    }
}

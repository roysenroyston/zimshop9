namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fiscalday : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.fiscaldays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceId = c.Int(nullable: false),
                        FiscalDayNo = c.Int(nullable: false),
                        OperationId = c.String(maxLength: 100),
                        FiscalStatus = c.String(),
                        IsOpen = c.Boolean(nullable: false),
                        DateOpened = c.DateTime(),
                        DateClosed = c.DateTime(),
                        AddedBy = c.Int(),
                        WarehouseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.fiscaldays");
        }
    }
}

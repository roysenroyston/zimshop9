namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class supplier : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Vendors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 100),
                        FullName = c.String(maxLength: 111),
                        Mobile = c.String(maxLength: 15),
                        Email = c.String(maxLength: 200),
                        Address = c.String(maxLength: 200),
                        About = c.String(),
                        JoinDate = c.DateTime(),
                        IsActive = c.Boolean(),
                        vatNumber = c.String(maxLength: 150),
                        WarehouseId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Purchase", "Vendor_Id", c => c.Int());
            CreateIndex("dbo.Purchase", "Vendor_Id");
            AddForeignKey("dbo.Purchase", "Vendor_Id", "dbo.Vendors", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Purchase", "Vendor_Id", "dbo.Vendors");
            DropIndex("dbo.Purchase", new[] { "Vendor_Id" });
            DropColumn("dbo.Purchase", "Vendor_Id");
            DropTable("dbo.Vendors");
        }
    }
}

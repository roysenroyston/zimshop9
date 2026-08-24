namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeinvoiceid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Purchase", "VendorUserId", "dbo.User");
            DropIndex("dbo.Purchase", new[] { "VendorUserId" });
            DropIndex("dbo.Purchase", new[] { "Vendor_Id" });
            DropColumn("dbo.Purchase", "VendorUserId");
            RenameColumn(table: "dbo.Purchase", name: "Vendor_Id", newName: "VendorUserId");
            AddColumn("dbo.Purchase", "User_Id", c => c.Int());
            AlterColumn("dbo.Purchase", "VendorUserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Purchase", "VendorUserId");
            CreateIndex("dbo.Purchase", "User_Id");
            AddForeignKey("dbo.Purchase", "User_Id", "dbo.User", "Id");
            DropColumn("dbo.Purchase", "invoiceid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Purchase", "invoiceid", c => c.Int(nullable: false));
            DropForeignKey("dbo.Purchase", "User_Id", "dbo.User");
            DropIndex("dbo.Purchase", new[] { "User_Id" });
            DropIndex("dbo.Purchase", new[] { "VendorUserId" });
            AlterColumn("dbo.Purchase", "VendorUserId", c => c.Int());
            DropColumn("dbo.Purchase", "User_Id");
            RenameColumn(table: "dbo.Purchase", name: "VendorUserId", newName: "Vendor_Id");
            AddColumn("dbo.Purchase", "VendorUserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Purchase", "Vendor_Id");
            CreateIndex("dbo.Purchase", "VendorUserId");
            AddForeignKey("dbo.Purchase", "VendorUserId", "dbo.User", "Id");
        }
    }
}

namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class jhgf : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sale", "Customers_Id", "dbo.Customers");
            DropIndex("dbo.Sale", new[] { "Customers_Id" });
            AddColumn("dbo.Customers", "BuyerRegisterName", c => c.String());
            AddColumn("dbo.Customers", "BuyerTradeName", c => c.String());
            AddColumn("dbo.Customers", "BuyerTIN", c => c.String());
            AddColumn("dbo.Customers", "PhoneNo", c => c.String());
            AddColumn("dbo.Customers", "Province", c => c.String());
            AddColumn("dbo.Customers", "Street", c => c.String());
            AddColumn("dbo.Customers", "HouseNo", c => c.String());
            AddColumn("dbo.Customers", "City", c => c.String());
            AddColumn("dbo.Customers", "JoinedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Customers", "Email", c => c.String());
            AlterColumn("dbo.Customers", "isActive", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Customers", "VATNumber", c => c.String());
            AlterColumn("dbo.Customers", "WarehouseId", c => c.Int(nullable: false));
            DropColumn("dbo.Sale", "Customers_Id");
            DropColumn("dbo.Customers", "UserName");
            DropColumn("dbo.Customers", "FullName");
            DropColumn("dbo.Customers", "Mobile");
            DropColumn("dbo.Customers", "Address");
            DropColumn("dbo.Customers", "About");
            DropColumn("dbo.Customers", "JoinDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "JoinDate", c => c.DateTime());
            AddColumn("dbo.Customers", "About", c => c.String());
            AddColumn("dbo.Customers", "Address", c => c.String(maxLength: 200));
            AddColumn("dbo.Customers", "Mobile", c => c.String(maxLength: 15));
            AddColumn("dbo.Customers", "FullName", c => c.String(maxLength: 111));
            AddColumn("dbo.Customers", "UserName", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Sale", "Customers_Id", c => c.Int());
            AlterColumn("dbo.Customers", "WarehouseId", c => c.Int());
            AlterColumn("dbo.Customers", "VATNumber", c => c.String(maxLength: 150));
            AlterColumn("dbo.Customers", "isActive", c => c.Boolean());
            AlterColumn("dbo.Customers", "Email", c => c.String(maxLength: 200));
            DropColumn("dbo.Customers", "JoinedDate");
            DropColumn("dbo.Customers", "City");
            DropColumn("dbo.Customers", "HouseNo");
            DropColumn("dbo.Customers", "Street");
            DropColumn("dbo.Customers", "Province");
            DropColumn("dbo.Customers", "PhoneNo");
            DropColumn("dbo.Customers", "BuyerTIN");
            DropColumn("dbo.Customers", "BuyerTradeName");
            DropColumn("dbo.Customers", "BuyerRegisterName");
            CreateIndex("dbo.Sale", "Customers_Id");
            AddForeignKey("dbo.Sale", "Customers_Id", "dbo.Customers", "Id");
        }
    }
}

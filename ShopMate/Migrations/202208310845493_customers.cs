namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
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
            
            AddColumn("dbo.Sale", "Customers_Id", c => c.Int());
            CreateIndex("dbo.Sale", "Customers_Id");
            AddForeignKey("dbo.Sale", "Customers_Id", "dbo.Customers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sale", "Customers_Id", "dbo.Customers");
            DropIndex("dbo.Sale", new[] { "Customers_Id" });
            DropColumn("dbo.Sale", "Customers_Id");
            DropTable("dbo.Customers");
        }
    }
}

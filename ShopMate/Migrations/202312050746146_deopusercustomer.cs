namespace ShopMate.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deopusercustomer : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sale", "CustomerUserId", "dbo.User");
            DropIndex("dbo.Sale", new[] { "CustomerUserId" });
            AddColumn("dbo.Sale", "User_Id", c => c.Int());
            CreateIndex("dbo.Sale", "User_Id");
            AddForeignKey("dbo.Sale", "User_Id", "dbo.User", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sale", "User_Id", "dbo.User");
            DropIndex("dbo.Sale", new[] { "User_Id" });
            DropColumn("dbo.Sale", "User_Id");
            CreateIndex("dbo.Sale", "CustomerUserId");
            AddForeignKey("dbo.Sale", "CustomerUserId", "dbo.User", "Id");
        }
    }
}

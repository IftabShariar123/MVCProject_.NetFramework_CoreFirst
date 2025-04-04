namespace Project_MVC_CF_Special.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class flajl : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Products", "CategoryID");
            AddForeignKey("dbo.Products", "CategoryID", "dbo.Categories", "CategoryID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "CategoryID", "dbo.Categories");
            DropIndex("dbo.Products", new[] { "CategoryID" });
        }
    }
}

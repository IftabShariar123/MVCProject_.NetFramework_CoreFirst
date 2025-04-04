namespace Project_MVC_CF_Special.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shariar : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RolePermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                        Permission = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RolePermissions");
        }
    }
}

namespace ManuelsUno.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GameCountEvents",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Game = c.String(),
                        Player = c.String(),
                        Count = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GameCountEvents");
        }
    }
}

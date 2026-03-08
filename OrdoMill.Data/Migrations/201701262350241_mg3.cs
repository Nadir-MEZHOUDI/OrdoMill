namespace OrdoMill.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mg3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Assures", "FullName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Assures", "FullName", c => c.String());
        }
    }
}

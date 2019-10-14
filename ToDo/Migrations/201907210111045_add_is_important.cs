namespace ToDo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_is_important : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ToDoes", "is_important", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ToDoes", "is_important");
        }
    }
}

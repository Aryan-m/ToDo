namespace ToDo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_row_number : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ToDoes", "row_number", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ToDoes", "row_number");
        }
    }
}

namespace ToDo.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ToDo.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            Add_User(context);
        }

        public void Add_User(ApplicationDbContext context)
        {
            var user = new ApplicationUser { UserName = "DefaultUser" };
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context)
                );
            manager.Create(user, "qwertyuiop");
        }
    }
}

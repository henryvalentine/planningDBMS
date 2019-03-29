using System.Collections.Generic;
using DPR_DataMigrationEngine.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DPR_DataMigrationEngine.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DPR_DataMigrationEngine.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Models.ApplicationDbContext context)
        {
            //r => r.Name == "AppAdmin")
            if (!context.Roles.Any())
            {
                //var store = new RoleStore<IdentityRole>(context);
                //var manager = new RoleManager<IdentityRole>(store);
                var roles = new List<IdentityRole> { new IdentityRole {Name = "Admin"}, new IdentityRole{Name = "DataEntry"},new IdentityRole{Name = "Supervisor"} };
                //manager.Create(role);
                roles.ForEach(m => context.Roles.AddOrUpdate(m));
            }

            if (!context.Roles.Any(M => M.Name == "Super_Admin"))
            {
                //var store = new RoleStore<IdentityRole>(context);
                //var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = "Super_Admin" };
                //manager.Create(role);
                context.Roles.AddOrUpdate(role);
            }

            if (!context.Users.Any(u => u.UserName == "admin"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser
                {
                    UserName = "admin",

                    UserInfo = new ApplicationDbContext.UserProfile
                    {

                        FirstName = "Planning",
                        LastName = "Admin",
                        Email = "planningadmin@dpr.gov.ng"
                    }
                };
                
               manager.CreateAsync(user, "admin_123");
               manager.AddToRole(user.Id, "Admin");
            }
            
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}

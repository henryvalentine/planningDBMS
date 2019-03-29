using Microsoft.AspNet.Identity.EntityFramework;

namespace DPR_DataMigrationEngine.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public virtual ApplicationDbContext.UserProfile UserInfo { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            :  base("LocalSqlServer", false) //base("LocalSqlServer")
        {
           

        }
        public System.Data.Entity.DbSet<UserProfile> UserInfo { get; set; }
        public class UserProfile
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public bool IsActive { get; set; }
        }

       
    }
}
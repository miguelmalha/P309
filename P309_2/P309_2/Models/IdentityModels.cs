using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace P309_2.Models
{
   public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Payment_Methods> Payment_Method { get; set; }
        public DbSet<Development_Areas> Development_Area { get; set; }
        public DbSet<Development_Stages> Development_Stage { get; set; }
        public DbSet<Companies> Company { get; set; }
        public DbSet<Contacts> Contact { get; set; }
        public DbSet<Projects> Project { get; set; }
        public DbSet<Project_Logs> Project_Log { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
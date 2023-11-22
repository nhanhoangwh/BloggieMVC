using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Seed Roles (User, Admin, SuperAdmin)

            var adminRoleId = "4ba84c3b-a97c-4725-a724-7787da119746";
            var superAdminRoleId = "624c6be0-d611-438a-9fda-7ffee5e03854";
            var userRoleId = "90951cd2-babb-4f75-be3c-ff01725067ef";

            var roles = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Name = "Admin",
                    NormalizedName = "Admin",
                    Id =  adminRoleId,
                    ConcurrencyStamp = adminRoleId
                },
                new IdentityRole()
                {
                    Name = "SuperAdmin",
                    NormalizedName = "SuperAdmin",
                    Id =  superAdminRoleId,
                    ConcurrencyStamp = superAdminRoleId
                },
                new IdentityRole()
                {
                    Name = "User",
                    NormalizedName = "User",
                    Id =  userRoleId,
                    ConcurrencyStamp = userRoleId
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);

            //Seed SuperAdminUser
            var superAdminId = "e52ef649-fb88-43df-9853-fab78ce01bc4";

            var superAdminUser = new IdentityUser
            {
                UserName = "superadmin@bloggie.com",
                Email = "superadmin@bloggie.com",
                NormalizedEmail = "superadmin@bloggie.com".ToUpper(),
                NormalizedUserName = "superadmin@bloggie.com".ToUpper(),
                Id = superAdminId
            };

            superAdminUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(superAdminUser, "Superadmin@123");

            builder.Entity<IdentityUser>().HasData(superAdminUser); 

            // Add All roles to SuperAdminUser
            var superAdminRoles = new List<IdentityUserRole<string>>()
            {
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = superAdminId,
                },
                new IdentityUserRole<string>
                {
                    RoleId = superAdminRoleId,
                    UserId = superAdminId,
                },
                new IdentityUserRole<string>
                {
                    RoleId = userRoleId,
                    UserId = superAdminId,
                }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(superAdminRoles);
        }
    }
}

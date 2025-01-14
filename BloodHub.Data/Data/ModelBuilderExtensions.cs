using Microsoft.EntityFrameworkCore;
using BloodHub.Shared.Entities;
using Microsoft.AspNetCore.Identity;

namespace BloodHub.Data.Data
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var passwordHasher = new PasswordHasher<User>();

            // Seed Roles
            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                new Role { Id = 2, Name = "Manager", NormalizedName = "MANAGER" },
                new Role { Id = 3, Name = "User", NormalizedName = "USER" }
            };
            modelBuilder.Entity<Role>().HasData(roles);

            // Seed Users
            var adminUser = new User
            {
                Id = 1,
                UserName = "superadmin",
                NormalizedUserName = "SUPERADMIN",
                FullName = "Quản trị hệ thống",
                Email = "superadmin@khotruyenmau.com",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin@123");

            var managerUser = new User
            {
                Id = 2,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                FullName = "Quản trị hệ thống",
                Email = "admin@khotruyenmau.com",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            managerUser.PasswordHash = passwordHasher.HashPassword(managerUser, "Admin@123");

            var staffUser = new User
            {
                Id = 3,
                UserName = "staff",
                NormalizedUserName = "STAFF",
                FullName = "Nhân viên",
                Email = "staff@khotruyenmau.com",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            staffUser.PasswordHash = passwordHasher.HashPassword(staffUser, "Admin@123");

            modelBuilder.Entity<User>().HasData(adminUser, managerUser, staffUser);

            // Assign Roles to Users
            var userRoles = new List<IdentityUserRole<int>>
            {
                new IdentityUserRole<int> { UserId = 1, RoleId = 1 }, // superadmin -> Admin
                new IdentityUserRole<int> { UserId = 2, RoleId = 2 }, // admin -> Manager
                new IdentityUserRole<int> { UserId = 3, RoleId = 3 }  // staff -> User
            };
            modelBuilder.Entity<IdentityUserRole<int>>().HasData(userRoles);
        }

    }
}
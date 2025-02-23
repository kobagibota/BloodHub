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
            var users = new List<User>
            {
                new User { Id = 1, UserName = "superadmin", NormalizedUserName = "SUPERADMIN", FullName = "Quản trị hệ thống", Email = "superadmin@khotruyenmau.com", EmailConfirmed = true, SecurityStamp = Guid.NewGuid().ToString(), IsActive = true },
                new User { Id = 2, UserName = "admin", NormalizedUserName = "ADMIN", FullName = "Quản trị hệ thống", Email = "admin@khotruyenmau.com", EmailConfirmed = true, SecurityStamp = Guid.NewGuid().ToString(), IsActive = true },
                new User { Id = 3, UserName = "staff", NormalizedUserName = "STAFF", FullName = "Nhân viên", Email = "staff@khotruyenmau.com", EmailConfirmed = true, SecurityStamp = Guid.NewGuid().ToString(), IsActive = true }
            };
            foreach (var user in users)
            {
                user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Admin@123"); // Hash mật khẩu
            }
            modelBuilder.Entity<User>().HasData(users);

            // Assign Roles to Users
            var userRoles = new List<UserRole>
            {
                new UserRole { UserId = 1, RoleId = 1 }, // superadmin -> Admin
                new UserRole { UserId = 2, RoleId = 2 }, // admin -> Manager
                new UserRole { UserId = 3, RoleId = 3 }  // staff -> User
            };
            modelBuilder.Entity<UserRole>().HasData(userRoles);
        }

    }
}
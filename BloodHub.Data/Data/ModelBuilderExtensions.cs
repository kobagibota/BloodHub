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
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Manager" },
                new Role { Id = 3, Name = "User" }
            };
            modelBuilder.Entity<Role>().HasData(roles);

            // Seed Users
            var users = new List<User>
            {
                new User { Id = 1, Username = "superadmin", Title = "Mr.", LastName = "Super", FirstName = "Admin", IsActive = true },
                new User { Id = 2, Username = "admin", Title = "Mr.", LastName = "Quản trị", FirstName = "Hệ thống", IsActive = true },
                new User { Id = 3, Username = "staff", Title = "CN.", LastName = "Nhân", FirstName = "Viên", IsActive = true }
            };
            foreach (var user in users)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123", workFactor: 12); // Hash mật khẩu với BCrypt
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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BloodHub.Shared.Entities;
using BloodHub.Data.Configurations;

namespace BloodHub.Data.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, Role, int>(options)
    {
        // Add-Migration InitialDB -OutputDir Data/Migrations

        #region Entities

        public DbSet<AuthToken> AuthTokens { get; set; }
        public DbSet<ActivityLog> ActitityLogs { get; set; }
        public DbSet<ChangeLog> ChangeLogs { get; set; }

        public DbSet<Crossmatch> Crossmatches { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Nursing> Nursings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<ShiftDetail> ShiftDetails { get; set; }
        public DbSet<ShiftUser> ShiftUsers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionDetail> TransactionDetails { get; set; }
        public DbSet<Ward> Wards { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            #region Configure using Fluent API

            // Cấu hình khóa chính cho các thực thể Identity
            builder.Entity<IdentityUserLogin<int>>().HasKey(l => new { l.LoginProvider, l.ProviderKey }); 
            builder.Entity<IdentityRoleClaim<int>>().HasKey(rc => rc.Id); 
            builder.Entity<IdentityUserRole<int>>().HasKey(ur => new { ur.UserId, ur.RoleId });
            builder.Entity<IdentityUserToken<int>>().HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

            // Cấu hình các thực thể Identity với tên bảng tùy chỉnh
            builder.Entity<IdentityUser<int>>().ToTable("AppUsers"); 
            builder.Entity<IdentityRole<int>>().ToTable("AppRoles"); 
            builder.Entity<IdentityUserRole<int>>().ToTable("AppUserRoles"); 
            builder.Entity<IdentityUserClaim<int>>().ToTable("AppUserClaims"); 
            builder.Entity<IdentityUserLogin<int>>().ToTable("AppUserLogins"); 
            builder.Entity<IdentityRoleClaim<int>>().ToTable("AppRoleClaims");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens"); 

            builder.ApplyConfiguration(new ActivityLogConfiguration());
            builder.ApplyConfiguration(new ChangeLogConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new AuthTokenConfiguration());

            builder.ApplyConfiguration(new ShiftConfiguration());
            builder.ApplyConfiguration(new ShiftDetailConfiguration());
            builder.ApplyConfiguration(new ShiftUserConfiguration());
            builder.ApplyConfiguration(new PatientConfiguration());
            builder.ApplyConfiguration(new InventoryConfiguration());
            builder.ApplyConfiguration(new CrossmatchConfiguration());
            builder.ApplyConfiguration(new TransactionConfiguration());
            builder.ApplyConfiguration(new TransactionDetailConfiguration());

            #endregion Configure using Fluent API
            
            // Data seeding gọi Seed nếu dùng extention
            builder.Seed();
        }
    }
}
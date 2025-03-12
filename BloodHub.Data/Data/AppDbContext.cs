using Microsoft.EntityFrameworkCore;
using BloodHub.Shared.Entities;
using BloodHub.Data.Configurations;

namespace BloodHub.Data.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        // Add-Migration InitialDB -OutputDir Data/Migrations

        #region Entities

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<AuthToken> AuthTokens { get; set; }

        public DbSet<ActivityLog> ActivityLogs { get; set; }
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
            base.OnModelCreating(builder);

            #region Configure using Fluent API
                        
            builder.ApplyConfiguration(new ActivityLogConfiguration());
            builder.ApplyConfiguration(new ChangeLogConfiguration());

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());

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
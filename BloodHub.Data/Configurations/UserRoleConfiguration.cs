using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using BloodHub.Shared.Entities;

namespace BloodHub.Data.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("AppUserRoles");
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });
        }
    }
}

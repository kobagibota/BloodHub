using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using BloodHub.Shared.Entities;

namespace BloodHub.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("AppUsers");
            builder.HasIndex(x => x.Username).IsUnique();
            builder.Property(x => x.Username).IsUnicode(false);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GetDate()").ValueGeneratedOnAdd();
            builder.Property(x => x.IsActive).HasDefaultValue(true).ValueGeneratedOnAdd();
            builder.Property(x => x.MustChangePassword).HasDefaultValue(true).ValueGeneratedOnAdd();
            builder.Property(x => x.IsOnDuty).HasDefaultValue(true).ValueGeneratedOnAdd();
        }
    }
}

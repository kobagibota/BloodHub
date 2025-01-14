using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using BloodHub.Shared.Entities;

namespace BloodHub.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(x => x.UserName).IsUnique();
            builder.Property(x => x.UserName).IsUnicode(false);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GetDate()").ValueGeneratedOnAdd();
        }
    }
}

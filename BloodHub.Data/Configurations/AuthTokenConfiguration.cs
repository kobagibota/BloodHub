using BloodHub.Shared.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BloodHub.Data.Configurations
{
    public class AuthTokenConfiguration : IEntityTypeConfiguration<AuthToken>
    {
        public void Configure(EntityTypeBuilder<AuthToken> builder)
        {
            builder.HasIndex(x => x.TokenHash).IsUnique();
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GetDate()").ValueGeneratedOnAdd();
        }
    }
}

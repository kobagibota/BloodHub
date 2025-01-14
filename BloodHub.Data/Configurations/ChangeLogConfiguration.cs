using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using BloodHub.Shared.Entities;

namespace BloodHub.Data.Configurations
{
    public class ChangeLogConfiguration : IEntityTypeConfiguration<ChangeLog>
    {
        public void Configure(EntityTypeBuilder<ChangeLog> builder)
        {
            builder.Property(x => x.ChangeType).IsRequired().HasConversion<string>();   // Lưu enum dưới dạng string
            builder.Property(x => x.ChangeTimestamp).HasDefaultValueSql("GetDate()").ValueGeneratedOnAdd();
        }
    }
}

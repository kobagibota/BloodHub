using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using BloodHub.Shared.Entities;

namespace BloodHub.Data.Configurations
{
    public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
    {
        public void Configure(EntityTypeBuilder<ActivityLog> builder)
        {
            builder.Property(x => x.Token).HasMaxLength(512);
            builder.Property(x => x.Activity).IsRequired().HasConversion<string>();   // Lưu enum dưới dạng string
            builder.Property(x => x.Timestamp).HasDefaultValueSql("GetDate()").ValueGeneratedOnAdd();
        }
    }
}

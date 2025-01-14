using BloodHub.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodHub.Data.Configurations
{
    public class ShiftUserConfiguration : IEntityTypeConfiguration<ShiftUser>
    {
        public void Configure(EntityTypeBuilder<ShiftUser> builder)
        {
            builder.HasKey(x => new { x.UserId, x.ShiftId });
        }
    }
}

using BloodHub.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodHub.Data.Configurations
{
    public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
    {
        public void Configure(EntityTypeBuilder<Shift> builder)
        {
            builder.HasOne(s => s.ReceivedShift).WithMany()
                .HasForeignKey(s => s.ReceivedShiftId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.UserHand).WithMany(u => u.ShiftsHanded)
                .HasForeignKey(s => s.HandBy).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.UserReceived).WithMany(u=>u.ShiftsReceived)
                .HasForeignKey(s => s.ReceivedBy).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

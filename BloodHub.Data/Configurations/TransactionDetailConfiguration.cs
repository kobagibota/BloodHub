using BloodHub.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodHub.Data.Configurations
{
    public class TransactionDetailConfiguration : IEntityTypeConfiguration<TransactionDetail>
    {
        public void Configure(EntityTypeBuilder<TransactionDetail> builder)
        {
            builder.HasOne(s => s.Transaction).WithMany(u => u.TransactionDetails)
                .HasForeignKey(s => s.TransactionId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using BloodHub.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodHub.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasOne(s => s.User).WithMany(u => u.Transactions)
                .HasForeignKey(s => s.ProcessedBy).OnDelete(DeleteBehavior.Restrict);
        }
    }
}

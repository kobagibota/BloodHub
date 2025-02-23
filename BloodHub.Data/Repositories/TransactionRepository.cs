using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;

namespace BloodHub.Data.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
    }
}

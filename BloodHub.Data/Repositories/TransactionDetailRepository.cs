using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;

namespace BloodHub.Data.Repositories
{
    public class TransactionDetailRepository : GenericRepository<TransactionDetail>, ITransactionDetailRepository
    {
        public TransactionDetailRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
    }
}

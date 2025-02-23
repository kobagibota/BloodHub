using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;

namespace BloodHub.Data.Repositories
{
    public class CrossmatchRepository : GenericRepository<Crossmatch>, ICrossmatchRepository
    {
        public CrossmatchRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
    }
}

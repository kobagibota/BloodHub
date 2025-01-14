using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;

namespace BloodHub.Data.Repositories
{
    public class ChangeLogRepository : GenericRepository<ChangeLog>, IChangeLogRepository
    {
        public ChangeLogRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
    }
}

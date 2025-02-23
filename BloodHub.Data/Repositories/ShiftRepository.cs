using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Request;
using System.Linq.Expressions;

namespace BloodHub.Data.Repositories
{
    public class ShiftRepository : GenericRepository<Shift>, IShiftRepository
    {
        public ShiftRepository(AppDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<bool> IsExists(Expression<Func<Shift, bool>> predicate)
        {
            return await ExistsAsync(predicate);
        }
    }
}

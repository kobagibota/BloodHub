using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;

namespace BloodHub.Data.Repositories
{
    public class ShiftUserRepository : GenericRepository<ShiftUser>, IShiftUserRepository
    {
        public ShiftUserRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
    }
}

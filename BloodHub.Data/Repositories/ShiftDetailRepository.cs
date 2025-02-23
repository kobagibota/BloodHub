using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;

namespace BloodHub.Data.Repositories
{
    public class ShiftDetailRepository : GenericRepository<ShiftDetail>, IShiftDetailRepository
    {
        public ShiftDetailRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
    }
}

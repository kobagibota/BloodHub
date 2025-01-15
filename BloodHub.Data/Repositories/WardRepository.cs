using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;

namespace BloodHub.Data.Repositories
{
    public class WardRepository : GenericRepository<Ward>, IWardRepository
    {
        public WardRepository(AppDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<bool> IsExists(int wardId, string wardName)
        {
            return await ExistsAsync(d => d.WardName == wardName && d.Id != wardId);
        }
    }
}

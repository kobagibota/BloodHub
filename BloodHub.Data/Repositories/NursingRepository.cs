using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;

namespace BloodHub.Data.Repositories
{
    public class NursingRepository : GenericRepository<Nursing>, INursingRepository
    {
        public NursingRepository(AppDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<bool> IsExists(int nursingId, string nursingName)
        {
            return await ExistsAsync(d => d.NursingName == nursingName && d.Id != nursingId);
        }
    }
}

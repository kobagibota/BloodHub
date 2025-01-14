using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;

namespace BloodHub.Data.Repositories
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(AppDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<bool> IsExists(int doctorId, string doctorName)
        {
            return await ExistsAsync(d => d.DoctorName == doctorName && d.Id != doctorId);
        }
    }
}

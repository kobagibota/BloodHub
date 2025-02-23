using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;
using System.Linq.Expressions;

namespace BloodHub.Data.Repositories
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(AppDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<bool> IsExists(Expression<Func<Doctor, bool>> predicate)
        {
            return await ExistsAsync(predicate);
        }
    }
}

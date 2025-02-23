using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;

namespace BloodHub.Data.Repositories
{
    public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
    }
}

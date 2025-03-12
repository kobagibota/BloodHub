using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloodHub.Data.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
               
        public async Task<Role?> GetByName(string roleName)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<bool> IsExists(string roleName)
        {
            return await ExistsAsync(r => r.Name == roleName);
        }
    }
}

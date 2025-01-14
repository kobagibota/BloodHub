using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;

namespace BloodHub.Data.Repositories
{
    public class UserRepository(AppDbContext dbContext) : GenericRepository<User>(dbContext), IUserRepository
    {
        public async Task<string> GetFullNameById(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            return user == null ? "Unknown" : user.FullName;
        }
    }
}

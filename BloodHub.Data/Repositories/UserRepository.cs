using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;
using System.Linq.Expressions;

namespace BloodHub.Data.Repositories
{
    public class UserRepository(AppDbContext dbContext) : GenericRepository<User>(dbContext), IUserRepository
    {
        public async Task<string> GetFullNameById(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            return user == null ? "Unknown" : user.FullName;
        }

        public async Task<bool> IsExists(Expression<Func<User, bool>> predicate)
        {
            return await ExistsAsync(predicate);
        }
    }
}

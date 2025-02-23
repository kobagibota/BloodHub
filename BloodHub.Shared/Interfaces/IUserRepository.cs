using BloodHub.Shared.Entities;
using System.Linq.Expressions;

namespace BloodHub.Shared.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<string> GetFullNameById(int id);

        Task<bool> IsExists(Expression<Func<User, bool>> predicate);
    }
}

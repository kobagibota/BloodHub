using BloodHub.Shared.Entities;
using System.Linq.Expressions;

namespace BloodHub.Shared.Interfaces
{
    public interface IShiftRepository : IGenericRepository<Shift>
    {
        Task<bool> IsExists(Expression<Func<Shift, bool>> predicate);
    }
}

using BloodHub.Shared.Entities;
using System.Linq.Expressions;

namespace BloodHub.Shared.Interfaces
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Task<bool> IsExists(Expression<Func<Doctor, bool>> predicate);
    }
}

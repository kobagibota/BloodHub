using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace BloodHub.Shared.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] include);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        Task<IEnumerable<T>> GetListByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] include);

        Task<T?> GetByIdAsync(int id);

        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] include);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

    }
}

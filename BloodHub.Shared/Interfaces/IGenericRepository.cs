using System.Linq.Expressions;

namespace BloodHub.Shared.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] include);

        Task<IEnumerable<T>> GetListByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] include);

        Task<T?> GetByIdAsync(int id);

        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

    }
}

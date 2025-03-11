using System.Linq.Expressions;

namespace CosmosBase
{
    public interface IEfRepository<T> : IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, int page = 0, int size = 10, params Expression<Func<T, object>>[] includes);
        Task<T> GetByIdCacheAsync(int id, CancellationToken? cancellation = default);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id, CancellationToken? cancellation);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken? cancellation = default);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken? cancellation = default);
        Task<T> QueryRawFirstOrDefaultAsync(string sql, CancellationToken? cancellation = default, params object[] parameters);
        Task<IEnumerable<T>> QueryRawListAsync(string sql, CancellationToken? cancellation = default, params object[] parameters);
        Task<bool> AnyAsync(CancellationToken? cancellation = default);
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace CosmosBase
{
    public class EfRepository<T> : IEfRepository<T> where T : BaseEntity
    {
        private readonly DbContext context;
        private readonly DbSet<T> dbSet;
        private readonly IHttpContextAccessor httpContextAccesor;
        private readonly ICacheRepository cache;

        public EfRepository(DbContext context, IHttpContextAccessor httpContextAccesor, ICacheRepository cache = null)
        {
            this.context = context;
            dbSet = context.Set<T>();
            this.httpContextAccesor = httpContextAccesor;
            this.cache = cache;
        }

        private Guid GetUserId()
        {
            string userId = httpContextAccesor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userId == null ? Guid.Empty.ToString() : userId);
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellation = default)
        {
            return await dbSet.FindAsync(id, cancellation);
        }

        public async Task<T> GetByIdCacheAsync(int id, CancellationToken cancellation = default)
        {
            if (cache == null)
            {
                return await dbSet.FindAsync(id, cancellation);

            }

            string cacheKey = $"{typeof(T).Name}:{id}";
            var cachedItem = await cache.GetAsync<T>(cacheKey, cancellation);
            if (cachedItem != null)
            {
                return cachedItem;

            }

            var item = await dbSet.FindAsync(id, cancellation);
            if (item != null)
            {
                await cache.SetAsync(cacheKey, item, cancellationToken: cancellation);
            }

            return item;
        }

        public async Task<IEnumerable<T>> GetListAsync(
            Expression<Func<T, bool>>? predicate = null,
            int page = 0,
            int size = 10,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = dbSet.AsNoTracking();

            foreach (var item in includes)
            {
                query = query.Include(item);

            }

            if (predicate != null)
            {
                query = query.Where(predicate);

            }

            return await query.Skip(size * page)
                .Take(size)
                .ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            dbSet.Update(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellation)
        {
            var entity = await GetByIdAsync(id, cancellation);
            dbSet.Update(entity);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
        {
            return await dbSet.FirstOrDefaultAsync(predicate, cancellationToken: cancellation);
        }
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellation = default)
        {
            return await dbSet.Where(predicate).ToListAsync(cancellation);
        }

        public async Task<T> QueryRawFirstOrDefaultAsync(string sql, CancellationToken cancellation = default, params object[] parameters)
        {
            return await dbSet.FromSqlRaw(sql, parameters).FirstOrDefaultAsync(cancellation);

        }

        public async Task<IEnumerable<T>> QueryRawListAsync(string sql, CancellationToken cancellation = default, params object[] parameters)
        {
            return await dbSet.FromSqlRaw(sql, parameters).AsNoTracking().ToListAsync(cancellation);
        }

        public async Task<bool> AnyAsync(CancellationToken cancellation = default)
        {
            return await dbSet.AnyAsync(cancellation);
        }
    }
}

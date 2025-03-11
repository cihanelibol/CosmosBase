using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CosmosBase
{
    public class DapperRepository<T> : IDapperRepository<T> where T : BaseEntity
    {
        private readonly DbContext context;
        private readonly IDbConnection conn;

        public DapperRepository(DbContext dbContext, IDbConnection conn)
        {
            this.context = dbContext;
            this.conn = conn;
        }

        public async Task<T> GetByIdAsync(int id, CancellationToken cancellation)
        {
            var sql = $"SELECT * FROM {typeof(T)} WHERE Id = @Id";

            return await conn.QueryFirstOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<IEnumerable<T>> GetListAsync(string sql, object[] parameters = null)
        {
            return await conn.QueryAsync<T>(sql, parameters);
        }
    }
}

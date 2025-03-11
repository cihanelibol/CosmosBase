using Microsoft.EntityFrameworkCore;

namespace CosmosBase
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        TContext Context { get; }
        IEfRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;
        IDapperRepository<TEntity> GetDapperRepository<TEntity>() where TEntity : BaseEntity;
        Task<int> SaveChangesAsync();
    }
}

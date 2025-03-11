namespace CosmosBase
{
    public interface IDapperRepository<T> : IRepository<T> where T: BaseEntity
    {
        Task<IEnumerable<T>> GetListAsync(string sql, object[] parameters = null);
    }
}

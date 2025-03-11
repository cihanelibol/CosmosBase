namespace CosmosBase
{
    public interface IRepository<T> where T: BaseEntity
    {
        Task<T> GetByIdAsync(int id, CancellationToken cancellation);
    }
}

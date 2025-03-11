namespace CosmosBase
{
    /// <summary>
    /// Implementing the disturbated cache like redis. This class written based on redis functions
    /// </summary>
    public interface ICacheRepository
    {

        T Get<T>(string key);
        Task<T> GetAsync<T>(string key, CancellationToken? cancellationToken = default);
        bool Set<T>(string key, T value, TimeSpan? expiry = null);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken? cancellationToken = default);
        bool Remove(string key);
        Task RemoveAsync(string key, CancellationToken? cancellationToken = default);
    }
}
using StackExchange.Redis;
using System.Text.Json;

namespace CosmosBase
{
    public class RedisRepository : ICacheRepository
    {
        private readonly IDatabase redisDb;
        private readonly IConnectionMultiplexer redisConnection;
        public RedisRepository(IConnectionMultiplexer redisConnection)
        {
            this.redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
            if (!this.redisConnection.IsConnected)
            {
                throw new InvalidOperationException("Redis connection could not build");

            }

            redisDb = redisConnection.GetDatabase();
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken? CancellationToken = default)
        {
            var cachedValue = await redisDb.StringGetAsync(key).ConfigureAwait(false);
            if (!cachedValue.HasValue)
                return default;

            return JsonSerializer.Deserialize<T>(cachedValue);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken? CancellationToken = default)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            await redisDb.StringSetAsync(key, serializedValue, expiry).ConfigureAwait(false);
        }

        public async Task RemoveAsync(string key, CancellationToken? CancellationToken = default)
        {
            await redisDb.KeyDeleteAsync(key).ConfigureAwait(false);
        }

        public T Get<T>(string key)
        {
            var cachedValue = redisDb.StringGet(key);
            if (!cachedValue.HasValue)
                return default;

            return JsonSerializer.Deserialize<T>(cachedValue);
        }

        public bool Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            return redisDb.StringSet(key, serializedValue, expiry);
        }

        private void EnsureConnection()
        {
            if (this.redisConnection.IsConnected)
            {
                throw new InvalidOperationException("Redis connection is not active");
            }
        }

        public bool Remove(string key)
        {
            return redisDb.KeyDelete(key);
        }
    }
}
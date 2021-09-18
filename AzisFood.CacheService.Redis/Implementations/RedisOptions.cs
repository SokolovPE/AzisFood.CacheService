using AzisFood.CacheService.Redis.Interfaces;

namespace AzisFood.CacheService.Redis.Implementations
{
    public class RedisOptions : IRedisOptions
    {
        public string ConnectionString { get; set; }
    }
}
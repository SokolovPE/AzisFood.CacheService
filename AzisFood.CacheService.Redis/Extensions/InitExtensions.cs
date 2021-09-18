using AzisFood.CacheService.Redis.Implementations;
using AzisFood.CacheService.Redis.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AzisFood.CacheService.Redis.Extensions
{
    public static class InitExtensions
    {
        public static void AddRedisSupport(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<RedisOptions>(configuration.GetSection(nameof(RedisOptions)));
            serviceCollection.AddSingleton<IRedisOptions>(sp =>
                sp.GetRequiredService<IOptions<RedisOptions>>().Value);
            serviceCollection.AddSingleton<IRedisCacheService, RedisCacheService>();
        }
    }
}
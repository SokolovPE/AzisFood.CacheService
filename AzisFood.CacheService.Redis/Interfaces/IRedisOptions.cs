namespace AzisFood.CacheService.Redis.Interfaces
{
    public interface IRedisOptions
    {
        string ConnectionString { get; set; }
    }
}
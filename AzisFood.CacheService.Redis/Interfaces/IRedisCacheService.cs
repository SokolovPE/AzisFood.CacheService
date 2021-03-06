using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace AzisFood.CacheService.Redis.Interfaces
{
    /// <summary>
    /// Service to operate cache in redis
    /// </summary>
    public interface IRedisCacheService
    {
        /// <summary>
        /// Redis connection
        /// </summary>
        static ConnectionMultiplexer Connection;
        
        /// <summary>
        /// Set value to redis cache
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <param name="value">The value to set</param>
        /// <param name="expiry">The expiry to set</param>
        /// <param name="flags">Flags of operation</param>
        /// <returns>Status of set operation</returns>
        Task<bool> SetRawAsync(RedisKey key, RedisValue value, TimeSpan? expiry,
            CommandFlags flags = CommandFlags.FireAndForget);

        /// <summary>
        /// Set value to redis cache
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <param name="value">The value to set</param>
        /// <param name="expiry">The expiry to set</param>
        /// <param name="flags">Flags of operation</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>Status of set operation</returns>
        Task<bool> SetAsync<T>(RedisKey key, T value, TimeSpan? expiry,
            CommandFlags flags = CommandFlags.FireAndForget);
        
        /// <summary>
        /// Get value from redis cache
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <param name="flags">Flags of operation</param>
        /// <returns>Value from redis</returns>
        Task<RedisValue> GetRawAsync(RedisKey key, CommandFlags flags = CommandFlags.None);
        
        /// <summary>
        /// Get value from redis cache
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <param name="flags">Flags of operation</param>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>Value from redis</returns>
        Task<T> GetAsync<T>(RedisKey key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Remove value from redis cache
        /// </summary>
        /// <param name="key">The key of the string</param>
        /// <param name="flags">Flags of operation</param>
        /// <returns>Status of del operation</returns>
        Task<bool> RemoveAsync(RedisKey key, CommandFlags flags = CommandFlags.FireAndForget);

        /// <summary>
        /// Set collection as hashset to redis cache
        /// </summary>
        /// <param name="value">Collection to be set</param>
        /// <param name="flags">Flags of operation</param>
        /// <typeparam name="T">Type of entity</typeparam>
        Task HashSetAsync<T>(IEnumerable<T> value,
            CommandFlags flags = CommandFlags.FireAndForget);

        /// <summary>
        /// Get collection from redis cache hashset
        /// </summary>
        /// <param name="flags">Flags of operation</param>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <returns>Collection of entity</returns>
        Task<IEnumerable<T>> HashGetAllAsync<T>(CommandFlags flags = CommandFlags.FireAndForget);

        /// <summary>
        /// Get hashset entry collection from redis cache
        /// </summary>
        /// <param name="key">Key of entry</param>
        /// <param name="flags">Flags of operation</param>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <returns>Entity value from hashset</returns>
        Task<T> HashGetAsync<T>(RedisValue key, CommandFlags flags = CommandFlags.FireAndForget) where T : class;

        /// <summary>
        /// Append entry to hashset
        /// </summary>
        /// <param name="value">Entity entry to be appended</param>
        /// <param name="flags">Flags of operation</param>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <returns>Status of set operation</returns>
        Task<bool> HashAppendAsync<T>(T value, CommandFlags flags = CommandFlags.FireAndForget);

        /// <summary>
        /// Remove entry from hashset
        /// </summary>
        /// <param name="key">Entity entry key to be removed</param>
        /// <param name="flags">Flags of operation</param>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <returns>Status of del operation</returns>
        Task<bool> HashRemoveAsync<T>(RedisValue key, CommandFlags flags = CommandFlags.FireAndForget);

        /// <summary>
        /// Remove entries from hashset
        /// </summary>
        /// <param name="keys">Entity entry keys to be removed</param>
        /// <param name="flags">Flags of operation</param>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <returns>Status of del operation</returns>
        Task<long> HashRemoveManyAsync<T>(RedisValue[] keys, CommandFlags flags = CommandFlags.FireAndForget);

        /// <summary>
        /// Remove all entries from hashset
        /// </summary>
        /// <param name="flags">Flags of operation</param>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <returns>Status of del operation</returns>
        Task<bool> HashDropAsync<T>(CommandFlags flags = CommandFlags.FireAndForget);
    }
}
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace jfYu.Core.Redis
{
    public interface IRedisService
    {
        /// <summary>
        /// redis client
        /// </summary>
        IConnectionMultiplexer Client { get; set; }

        /// <summary>
        /// set
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="timeSpan">expire time</param>
        /// <returns>success/failed</returns>
        Task<bool> AddAsync<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// get
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>value</returns>
        Task<string?> GetAsync(string key);

        /// <summary>
        /// get
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="key">key</param>
        /// <returns>value</returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// lock
        /// </summary>
        /// <param name="key">ley</param>
        /// <param name="expiry">expire time</param>
        /// <returns>success/failed</returns>
        Task<bool> LockAsync(string key, TimeSpan? expiry);

        /// <summary>
        /// unlock
        /// </summary>
        /// <param name="key">ley</param>
        /// <param name="value">token</param>
        /// <returns>success/failed</returns>
        Task<bool> UnLockAsync(string key);

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>success/failed</returns>
        Task<bool> RemoveAsync(string key);
    }
}
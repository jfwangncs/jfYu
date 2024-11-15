using jfYu.Core.Redis.Serializer;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jfYu.Core.Redis.Interface
{
    public partial interface IRedisService
    {
        /// <summary>
        /// Redis client
        /// </summary>
        IConnectionMultiplexer Client { get; }

        /// <summary>
        /// Database 
        /// </summary>
        IDatabase Database { get; }

        /// <summary>
        /// Gets the instance of <see cref="ISerializer" />
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// Verify that the specified cache key exists
        /// </summary>
        /// <param name="key">The cache key</param>
        /// <param name="flag">Behaviour markers associated with a given command</param>
        /// <returns>True if the key is present into Redis. Othwerwise False</returns>
        Task<bool> ExistsAsync(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>success/failed</returns>
        Task<bool> RemoveAsync(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        ///     Removes all specified keys
        /// </summary>
        /// <param name="keys">The cache keys.</param>
        /// <param name="flag">Behaviour markers associated with a given command</param>
        /// <returns>The numnber of items removed.</returns>
        Task<long> RemoveAllAsync(List<string> keys, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// get
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="key">key</param>
        /// <returns>value</returns>
        Task<T?> GetAsync<T>(string key, CommandFlags flag = CommandFlags.None);

        /// <summary>
        /// get
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="key">key</param>
        /// <param name="expiresIn">Time till the object expires.</param>
        /// <returns>value</returns>
        Task<T?> GetAsync<T>(string key, TimeSpan expiresIn, CommandFlags flag = CommandFlags.None);


        /// <summary>
        /// set
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="timeSpan">expire time</param>
        /// <param name="when">The condition (Always is the default value).</param>
        /// <returns>success/failed</returns>
        Task<bool> AddAsync<T>(string key, T value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flag = CommandFlags.None);

    }
}
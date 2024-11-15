using jfYu.Core.Redis.Interface;
using jfYu.Core.Redis.Serializer;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jfYu.Core.Redis.Implementation
{
    public partial class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _client;
        private readonly IDatabase _database;
        private readonly ISerializer _serializer;

        /// <summary>
        /// ConnectionMultiplexer
        /// </summary>
        public IConnectionMultiplexer Client => _client;

        /// <summary>
        /// database
        /// </summary>
        public IDatabase Database => _database;

        /// <summary>
        /// Serializer
        /// </summary>
        public ISerializer Serializer => _serializer;

        public RedisService(IOptions<RedisConfiguration> redisConfiguration, IConnectionMultiplexer client, ISerializer serializer)
        {
            try
            {
                _client = client;
                if (string.IsNullOrEmpty(redisConfiguration.Value.Prefix))
                    _database = Client.GetDatabase(redisConfiguration.Value.DbIndex);
                else
                    _database = Client.GetDatabase(redisConfiguration.Value.DbIndex).WithKeyPrefix(redisConfiguration.Value.Prefix);
                _serializer = serializer;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Task<bool> ExistsAsync(string key, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            return _database.KeyExistsAsync(key, flag);
        }

        public async Task<bool> RemoveAsync(string key, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            return await _database.KeyDeleteAsync(key, flag);
        }
        public Task<long> RemoveAllAsync(List<string> keys, CommandFlags flags = CommandFlags.None)
        {
            if (keys == null || keys.Count == 0)
                throw new ArgumentException(nameof(keys));

            var redisKeys = keys.Select(q => (RedisKey)q); 
            return _database.KeyDeleteAsync(redisKeys.ToArray(), flags);
        }
        public async Task<T?> GetAsync<T>(string key, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            var valueBytes = await _database.StringGetAsync(key, flag);
            return !valueBytes.HasValue ? default : Serializer.Deserialize<T>(valueBytes!);
        }

        public async Task<T?> GetAsync<T>(string key, TimeSpan expiresin, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            var result = await GetAsync<T>(key, flag);

            if (!EqualityComparer<T?>.Default.Equals(result, default))
                await _database.KeyExpireAsync(key, expiresin);

            return result;
        }

        public async Task<bool> AddAsync<T>(string key, T value, TimeSpan? expiresIn = null, When when = When.Always, CommandFlags flag = CommandFlags.None)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            var entryBytes = _serializer.Serialize(value);
            return await _database.StringSetAsync(key, entryBytes, expiresIn, when, flag);
        }


    }
}

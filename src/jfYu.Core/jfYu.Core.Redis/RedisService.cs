using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace jfYu.Core.Redis
{
    public class RedisService : IRedisService
    {


        private IConnectionMultiplexer _client;


        private IDatabase _database;

        /// <summary>
        /// ConnectionMultiplexer
        /// </summary>
        public IConnectionMultiplexer Client { get { return _client; } set { _client = value; _database = value.GetDatabase(); } }

        /// <summary>
        /// database
        /// </summary>
        public IDatabase Database { get { return _database; } }

        public RedisService(RedisConfiguration redisConfiguration)
        {
            try
            {
                var configurationOptions = new ConfigurationOptions()
                {
                    Password = redisConfiguration.Password,
                    ConnectTimeout = redisConfiguration.Timeout,
                    KeepAlive = 60,
                    AbortOnConnectFail = false,
                    Ssl = redisConfiguration.Ssl,
                };
                foreach (var endPoint in redisConfiguration.EndPoints)
                {
                    configurationOptions.EndPoints.Add(endPoint.Host, endPoint.Port);
                }
                _client = ConnectionMultiplexer.Connect(configurationOptions);
                _database = Client.GetDatabase(redisConfiguration.DbIndex);
            }
            catch (Exception ex)
            {
                throw new Exception($"error redis config:{ex.Message}-{ex.InnerException?.Message}");
            }
        }

        public async Task<bool> AddAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return await _database.StringSetAsync(key, JsonConvert.SerializeObject(value), expiry);
        }

        public async Task<string?> GetAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            var value = await Database.StringGetAsync(key);

            if (value.IsNull)
                return default;

            return JsonConvert.DeserializeObject<string>(value.ToString());
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var value = await Database.StringGetAsync(key);

            if (value.IsNull)
                return default;

            return JsonConvert.DeserializeObject<T>(value.ToString());
        }

        private static readonly RedisValue LockToken = Environment.MachineName;

        public async Task<bool> LockAsync(string key, TimeSpan? expiry)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            expiry ??= TimeSpan.FromMinutes(1);

            return await _database.LockTakeAsync(key, LockToken, expiry.Value);

        }
        public async Task<bool> UnLockAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return await _database.LockReleaseAsync(key, LockToken);
        }
        public async Task<bool> RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));

            return await _database.KeyDeleteAsync(key);
        }
    }
}

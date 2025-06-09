using jfYu.Core.Cache.Configurations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace jfYu.Core.Cache
{
    public class CacheService(IDistributedCache cache, IOptions<BaseOptions> options,ILogger<CacheService>? logger = null) : ICacheService
    {
        private readonly IDistributedCache _cache = cache;
        private readonly ILogger<CacheService>? _logger = logger;
        private readonly BaseOptions _options = options.Value;

        public async Task AddAsync<T>(string key, T value, TimeSpan? expiry)
        {
            if (_options.EnableLogging)
                _logger?.LogTrace("Add Cache:{key}", key);
            HandleKey(key);
            DistributedCacheEntryOptions cacheEntryOptions = new();
            if (_options.DefaultExpiration > 0)
            {
                expiry ??= TimeSpan.FromSeconds(_options.DefaultExpiration);
                cacheEntryOptions.SetAbsoluteExpiration(expiry.Value);
            }
            await _cache.SetStringAsync(key, JsonConvert.SerializeObject(value), cacheEntryOptions);
        }

        public async Task<string?> GetAsync(string key)
        {
            if (_options.EnableLogging)
                _logger?.LogTrace("Get Cache:{key}", key);
            HandleKey(key);
            var value = await _cache.GetStringAsync(key);
            if (value == null)
                return default;
            return JsonConvert.DeserializeObject<string>(value);
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            if (_options.EnableLogging)
                _logger?.LogTrace("Get Cache:{key}", key);
            var value = await _cache.GetStringAsync(key);
            if (value == null)
                return default;
            return JsonConvert.DeserializeObject<T>(value);
        }

        public async Task<int?> GetIntAsync(string key)
        {
            if (_options.EnableLogging)
                _logger?.LogTrace("Get Int Cache:{key}", key);
            HandleKey(key);
            var value = await _cache.GetStringAsync(key);
            if (value == null)
                return default;
            return JsonConvert.DeserializeObject<int>(value);
        }

        public async Task RemoveAsync(string key)
        {
            if (_options.EnableLogging)
                _logger?.LogTrace("Remove Cache:{key}", key);
            HandleKey(key);
            await _cache.RemoveAsync(key);
        }

        public static string HandleKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            return key;
        }
    }
}

using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace jfYu.Core.Cache
{
    public class CacheService(IDistributedCache cache) : ICacheService
    {

        private readonly IDistributedCache _cache = cache;

        public async Task AddAsync<T>(string key, T value, TimeSpan? expiry)
        {
            HandleKey(key);

            expiry ??= TimeSpan.FromMinutes(1);

            DistributedCacheEntryOptions cacheEntryOptions = new();
            cacheEntryOptions.SetSlidingExpiration(expiry.Value);
            await _cache.SetStringAsync(key, JsonConvert.SerializeObject(value), cacheEntryOptions);

        }

        public async Task<string?> GetAsync(string key)
        {
            HandleKey(key);

            var value = await _cache.GetStringAsync(key);

            if (value == null)
                return default;

            return JsonConvert.DeserializeObject<string>(value);
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            HandleKey(key);

            var value = await _cache.GetStringAsync(key);

            if (value == null)
                return default;

            return JsonConvert.DeserializeObject<T>(value);
        }

        public async Task<int?> GetIntAsync(string key)
        {
            HandleKey(key);

            var value = await _cache.GetStringAsync(key);

            if (value == null)
                return default;

            var intValue = JsonConvert.DeserializeObject<string>(value);

            if (intValue == null)
                return default;

            return int.Parse(intValue);
        }

        public async Task RemoveAsync(string key)
        {
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

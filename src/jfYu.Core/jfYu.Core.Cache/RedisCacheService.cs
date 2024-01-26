using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jfYu.Core.Cache
{
    public class CacheService(IDistributedCache cache) : ICacheService
    {
        private readonly List<string> _keys = [];
        private readonly IDistributedCache _cache = cache;

        #region add cache
        /// <summary>
        /// add
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expiration">expiretime,default:now add 1 min.</param>  
        public async Task AddAsync<T>(string key, T value, DateTime? expiration)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (expiration == null)
                expiration = DateTime.Now.AddMinutes(1);
            var response = JsonConvert.SerializeObject(value);
            DistributedCacheEntryOptions cacheEntryOptions = new()
            {
                AbsoluteExpiration = expiration.Value
            };
            await _cache.SetStringAsync(key, response, cacheEntryOptions);
            _keys.Add(key);
        }

        /// <summary>
        /// add
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="seconds">expire seconds,default:60s</param> 
        public async Task AddAsync<T>(string key, T value, int seconds = 60)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var response = JsonConvert.SerializeObject(value);
            DistributedCacheEntryOptions cacheEntryOptions = new();
            cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromSeconds(seconds));
            await _cache.SetStringAsync(key, response, cacheEntryOptions);
            _keys.Add(key);
        }

        #endregion

        #region remove cache

        /// <summary>
        /// remove
        /// </summary>
        /// <param name="key">key</param> 
        public async Task RemoveAsync(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            await _cache.RemoveAsync(key);
            _keys.Remove(key);
        }

        /// <summary>
        /// remove all 
        /// </summary> 
        public async Task RemoveAllAsync()
        {
            foreach (var key in _keys)
            {
                if (!string.IsNullOrEmpty(key))
                    await _cache.RemoveAsync(key);
            }
            _keys.Clear();
        }

        #endregion

        #region get cache

        /// <summary>
        /// get
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="key">key</param>
        /// <returns>value</returns> 
        public async Task<T> GetAsync<T>(string key) where T : class
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var value = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(value))
                return default;

            return JsonConvert.DeserializeObject<T>(value);
        }

        /// <summary>
        /// get
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>int value</returns> 
        public async Task<int> GetIntAsync(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var value = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(value))
                throw new NullReferenceException();

            return int.Parse(JsonConvert.DeserializeObject<string>(value));
        }

        /// <summary>
        /// get
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>string value</returns>
        public async Task<string> GetAsync(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var value = await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(value))
                return default;

            return JsonConvert.DeserializeObject<string>(value);
        }
        #endregion
    }
}

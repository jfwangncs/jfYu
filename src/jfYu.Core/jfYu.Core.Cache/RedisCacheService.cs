using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jfYu.Core.Cache
{
    public class CacheService
    {
        private static readonly List<string> _keys = new List<string>();
        private IDistributedCache _cache;
        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        #region 添加缓存
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiration">过期时间</param> 
        /// <returns>是否成功</returns>
        public async Task AddAsync<T>(string key, T value, DateTime? expiration)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (expiration == null)
                expiration = DateTime.Now.AddMinutes(1);
            var response = JsonConvert.SerializeObject(value);
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.AbsoluteExpiration = expiration.Value;
            await _cache.SetStringAsync(key, response, cacheEntryOptions);
            _keys.Add(key);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="seconds">过期秒数 默认60秒</param> 
        /// <returns>是否成功</returns>
        public async Task AddAsync<T>(string key, T value, int seconds = 60)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            var response = JsonConvert.SerializeObject(value);
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromSeconds(seconds));
            await _cache.SetStringAsync(key, response, cacheEntryOptions);
            _keys.Add(key);
        }

        #endregion

        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task RemoveAsync(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            await _cache.RemoveAsync(key);
            _keys.Remove(key);
        }

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns></returns>
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

        #region 获取缓存

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
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
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
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
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
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

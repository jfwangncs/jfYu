using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace jfYu.Core.Cache
{
    public class MemoryCacheService : CacheBase, ICacheService
    {
        protected IMemoryCache MemoryCache;

        public MemoryCacheService(IMemoryCache cache, CacheConfig cacheConfig) : base(cacheConfig)
        {
            CacheType = CacheType.Memory;
            MemoryCache = cache;
        }


        #region 判断缓存

        public bool Has(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return MemoryCache.TryGetValue(GetKey(key), out _);
        }

        public async Task<bool> HasAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return await Task.FromResult(MemoryCache.TryGetValue(GetKey(key), out _));
        }

        #endregion

        #region 添加缓存
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        public bool Add(string key, object value, bool IsNx = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (Has(key) && IsNx)
                return false;
            MemoryCache.Set(GetKey(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
            return Has(key);
        }

        /// <summary>
        /// 添加缓存（异步方式）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        public async Task<bool> AddAsync(string key, object value, bool IsNx = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (await HasAsync(key) && IsNx)
                return false;
            await Task.FromResult(MemoryCache.Set(GetKey(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value))));
            return await HasAsync(key);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiration">过期时间</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        public bool Add(string key, object value, TimeSpan expiration, bool IsNx = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (Has(key) && IsNx)
                return false;
            MemoryCache.Set(GetKey(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiration));

            return Has(key);

        }

        /// <summary>
        /// 添加缓存（异步方式）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiration">过期时间</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        public async Task<bool> AddAsync(string key, object value, TimeSpan expiration, bool IsNx = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (await HasAsync(key) && IsNx)
                return false;
            await Task.FromResult(MemoryCache.Set(GetKey(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiration)));

            return await HasAsync(key);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="seconds">过期秒数</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        public bool Add(string key, object value, int seconds, bool IsNx = false)
        {

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (Has(key) && IsNx)
                return false;
            MemoryCache.Set(GetKey(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(seconds)));
            return Has(key);
        }

        /// <summary>
        /// 添加缓存（异步方式）
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="seconds">过期秒数</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        public async Task<bool> AddAsync(string key, object value, int seconds, bool IsNx = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (await HasAsync(key) && IsNx)
                return false;
            await Task.FromResult(MemoryCache.Set(GetKey(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(seconds))));
            return await HasAsync(key);
        }
        #endregion

        #region 删除缓存

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public bool Remove(string key)
        {

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (!Has(key))
                return false;
            MemoryCache.Remove(GetKey(key));

            return !Has(key);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task<bool> RemoveAsync(string key)
        {

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (!await HasAsync(key))
                return false;
            MemoryCache.Remove(GetKey(key));
            return !await HasAsync(key);
        }
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns></returns>
        public IEnumerable<string> RemoveRange(IEnumerable<string> keys)
        {
            var failKeys = new List<string>();
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            foreach (var item in keys)
            {
                if (!Has(item) || !Remove(item))
                    failKeys.Add(item);
            }
            return failKeys;
        }

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> RemoveRangeAsync(IEnumerable<string> keys)
        {
            var failKeys = new List<string>();
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            foreach (var item in keys)
            {
                if (!await HasAsync(item) || !await RemoveAsync(item))
                    failKeys.Add(item);
            }
            return failKeys;
        }

        #endregion

        #region 获取缓存

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public T Get<T>(string key) where T : class
        {

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var value = MemoryCache.Get(GetKey(key));
            if (value == null)
                return null;
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(value as byte[]));
        }


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key) where T : class
        {

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            await Task.Delay(1);
            var value = MemoryCache.Get(GetKey(key));
            if (value == null)
                return null;
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(value as byte[]));
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var value = MemoryCache.Get(GetKey(key));
            if (value == null)
                return null;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(value as byte[]));
        }


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task<object> GetAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            await Task.Delay(1);
            var value = MemoryCache.Get(GetKey(key));
            if (value == null)
                return null;
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(value as byte[]));
        }


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public int GetInt(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (int.TryParse(GetString(key), out int n))
                return n;
            else
                throw new FormatException("转换Int类型失败。");
        }


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task<int> GetIntAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            await Task.Delay(1);
            if (int.TryParse(await GetStringAsync(key), out int n))
                return n;
            else
                throw new FormatException("转换Int类型失败。");
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public string GetString(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var value = MemoryCache.Get(GetKey(key));
            if (value == null)
                return null;
            return JsonConvert.DeserializeObject<string>(Encoding.UTF8.GetString(value as byte[]));
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            await Task.Delay(1);
            var value = MemoryCache.Get(GetKey(key));
            if (value == null)
                return null;
            return JsonConvert.DeserializeObject<string>(Encoding.UTF8.GetString(value as byte[]));
        }
        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns></returns>
        public IDictionary<string, object> GetRange(IEnumerable<string> keys)
        {

            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            var dict = new Dictionary<string, object>();
            foreach (var item in keys)
            {
                dict.Add(item, Get(item));
            }

            return dict;
        }

        /// <summary>
        /// 获取缓存集合
        /// </summary>
        /// <param name="keys">缓存Key集合</param>
        /// <returns></returns>
        public async Task<IDictionary<string, object>> GetRangeAsync(IEnumerable<string> keys)
        {

            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            var dict = new Dictionary<string, object>();
            foreach (var item in keys)
            {
                dict.Add(item, await GetAsync(item));
            }

            return dict;
        }
        #endregion

        #region 修改缓存

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <returns></returns>
        public bool Replace(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!Has(key) || !Remove(key))
                return false;

            return Add(key, value);
        }


        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <returns></returns>
        public async Task<bool> ReplaceAsync(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!await HasAsync(key) || !await RemoveAsync(key))
                return false;

            return await AddAsync(key, value);
        }

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <param name="expiration">过期时间</param>
        /// <returns></returns>
        public bool Replace(string key, object value, TimeSpan expiration)
        {


            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!Has(key) || !Remove(key))
                return false;
            return Add(key, value, expiration);
        }

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <param name="expiration">过期时间</param>
        /// <returns></returns>
        public async Task<bool> ReplaceAsync(string key, object value, TimeSpan expiration)
        {


            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!await HasAsync(key) || !await RemoveAsync(key))
                return false;
            return await AddAsync(key, value, expiration);
        }

        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <param name="seconds">过期秒数</param>
        /// <returns></returns>
        public bool Replace(string key, object value, int seconds)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!Has(key) || !Remove(key))
                return false;

            return Add(key, value, seconds);
        }


        /// <summary>
        /// 修改缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">新的缓存Value</param>
        /// <param name="seconds">过期秒数</param>
        /// <returns></returns>
        public async Task<bool> ReplaceAsync(string key, object value, int seconds)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!await HasAsync(key) || !await RemoveAsync(key))
                return false;

            return await AddAsync(key, value, seconds);
        }
        #endregion



    }
}

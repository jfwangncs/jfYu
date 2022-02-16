using jfYu.Core.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace jfYu.Core.Cache
{
    public class RedisCacheService : CacheBase, ICache
    {

        public RedisConfiguration RedisConfiguration { get; }
        private IDatabase RedisCache { get; }


        public RedisCacheService(CacheConfig cacheConfig) : base(cacheConfig)
        {
            try
            {
                CacheType = CacheType.Redis;
                RedisConfiguration = AppConfig.Configuration.GetSection("Redis").Get<RedisConfiguration>();
                var configurationOptions = new ConfigurationOptions()
                {
                    Password = RedisConfiguration.Password,
                    ConnectTimeout = RedisConfiguration.Timeout,
                    KeepAlive = 60,
                    AbortOnConnectFail = false
                };
                foreach (var endPoint in RedisConfiguration.EndPoints)
                {
                    configurationOptions.EndPoints.Add(endPoint.Host, endPoint.Port);
                }
                var redisClient = ConnectionMultiplexer.Connect(configurationOptions);
                RedisCache = redisClient.GetDatabase(RedisConfiguration.DbIndex);
            }
            catch (Exception ex)
            {
                throw new Exception($"错误的redis配置:{ex.Message}-{ex.StackTrace}");
            }
        }



        #region 判断缓存

        public bool Has(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return RedisCache.KeyExists(GetKey(key));
        }

        public async Task<bool> HasAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return await RedisCache.KeyExistsAsync(GetKey(key));
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
            return RedisCache.StringSet(GetKey(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
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
            return RedisCache.StringSet(GetKey(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), expiration);
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
            return RedisCache.StringSet(GetKey(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), TimeSpan.FromSeconds(seconds));

        }

        /// <summary>
        /// 添加缓存
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
            return await RedisCache.StringSetAsync(GetKey(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)));
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="expiration">过期时间</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        public async Task<bool> AddAsync(string key, object value, TimeSpan expiration, bool IsNx = false)
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
            return await RedisCache.StringSetAsync(GetKey(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), expiration);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <param name="value">缓存Value</param>
        /// <param name="seconds">过期秒数</param>
        /// <param name="IsNx">不存在该key添加缓存，否则失败，默认不开启</param>
        /// <returns>是否成功</returns>
        public async Task<bool> AddAsync(string key, object value, int seconds, bool IsNx = false)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (await HasAsync(key)&&IsNx)
                return false;
            return await RedisCache.StringSetAsync(GetKey(key), Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value)), TimeSpan.FromSeconds(seconds));
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
            return RedisCache.KeyDelete(GetKey(key));
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
            return await RedisCache.KeyDeleteAsync(GetKey(key));
        }

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns></returns>
        public IEnumerable<string> RemoveRange(IEnumerable<string> keys)
        {
            List<string> FailKeys = new List<string>();
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            foreach (var key in keys)
            {
                if (!Remove(key))
                    FailKeys.Add(key);
            }
            return FailKeys;
        }

        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="key">缓存Key集合</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> RemoveRangeAsync(IEnumerable<string> keys)
        {
            List<string> FailKeys = new List<string>();

            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            foreach (var key in keys)
            {
                if (!await RemoveAsync(key))
                    FailKeys.Add(key);
            }
            return FailKeys;
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
            var value = RedisCache.StringGet(GetKey(key));
            if (!value.HasValue)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(value);
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
            var value = await RedisCache.StringGetAsync(GetKey(key));
            if (!value.HasValue)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<T>(value);
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
            var value = RedisCache.StringGet(GetKey(key));
            if (!value.HasValue)
            {
                return null;
            }

            return JsonConvert.DeserializeObject(value);
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
            var value = await RedisCache.StringGetAsync(GetKey(key));
            if (!value.HasValue)
            {
                return null;
            }

            return JsonConvert.DeserializeObject(value);
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
            var value = RedisCache.StringGet(GetKey(key));
            if (!value.HasValue)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<string>(value);
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
            var value = await RedisCache.StringGetAsync(GetKey(key));
            if (!value.HasValue)
            {
                return default;
            }
            return JsonConvert.DeserializeObject<string>(value);
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

            foreach (var key in keys)
            {
                dict.Add(key, Get(key));
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

            foreach (var key in keys)
            {
                dict.Add(key, await GetAsync(key));
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

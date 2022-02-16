using jfYu.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace jfYu.Core.Redis
{
    public class RedisService
    {
        /// <summary>
        /// redis连接配置
        /// </summary>
        public RedisConfiguration RedisConfiguration { get; }

        /// <summary>
        /// redis具体操作库
        /// </summary>
        public IDatabase Database { get; }

        /// <summary>
        /// 客户端
        /// </summary>
        public ConnectionMultiplexer Client { get; set; }

        public RedisService()
        {
            try
            {
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
                Client = ConnectionMultiplexer.Connect(configurationOptions);
                Database = Client.GetDatabase(RedisConfiguration.DbIndex);
            }
            catch (Exception ex)
            {
                throw new Exception($"错误的redis配置:{ex.Message}-{ex.StackTrace}");
            }
        }

        public T Get<T>(string key)
        {
            return Deserialize<T>(Database.StringGet(key));
        }
        public object Get(string key)
        {
            return Deserialize<object>(Database.StringGet(key));
        }
        public async Task<T> GetAsync<T>(string key)
        {
            return Deserialize<T>(await Database.StringGetAsync(key));
        }
        public async Task<object> GetAsync(string key)
        {
            return Deserialize<object>(await Database.StringGetAsync(key));
        }
        public bool Remove(string key)
        {
            return Database.KeyDelete(key);
        }
        public async Task<bool> RemoveAsync(string key)
        {
            return await Database.KeyDeleteAsync(key);
        }
        public bool Set(string key, object value)
        {
            return Database.StringSet(key, Serialize(value));
        }
        public async Task<bool> SetAsync(string key, object value)
        {
            return await Database.StringSetAsync(key, Serialize(value));
        }
        public bool Set(string key, object value, TimeSpan timeSpan)
        {
            return Database.StringSet(key, Serialize(value), timeSpan);
        }
        public async Task<bool> SetAsync(string key, object value, TimeSpan timeSpan)
        {
            return await Database.StringSetAsync(key, Serialize(value), timeSpan);
        }

        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <param name="key">锁key</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns>是否成功获取锁，解锁value</returns>
        public (bool, string) Lock(string key, TimeSpan timeSpan)
        {
            string value = Guid.NewGuid().ToString("N");
            var flag = Database.StringSet(key, value, timeSpan, When.NotExists, CommandFlags.None);
            return (flag, value);
        }

        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <param name="key">锁key</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns>是否成功获取锁，解锁value</returns>
        public async Task<(bool, string)> LockAsync(string key, TimeSpan timeSpan)
        {
            string value = Guid.NewGuid().ToString("N");
            var flag = await Database.StringSetAsync(key, value, timeSpan, When.NotExists, CommandFlags.None);
            return (flag, value);
        }

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="key">锁key</param>
        /// <param name="value">解锁value</param>
        /// <returns>是否成功</returns>
        public bool UnLock(string key, string value)
        {
            if (Get(key).ToString() == value)
                return Remove(key);
            else
                return false;
        }

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="key">锁key</param>
        /// <param name="value">解锁value</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UnLockAsync(string key, string value)
        {
            var kv = await GetAsync(key);
            if (kv.ToString() == value)
                return await RemoveAsync(key);
            else
                return false;
        }
        byte[] Serialize(object t)
        {
            if (t == null)
            {
                return null;
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var str = JsonConvert.SerializeObject(t, settings);
            byte[] objectDataAsStream = Encoding.UTF8.GetBytes(str);
            return objectDataAsStream;
        }

        T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default;
            }

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            var json = Encoding.UTF8.GetString(stream);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}

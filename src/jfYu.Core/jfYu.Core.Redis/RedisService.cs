using jfYu.Core.Common.Configurations;
using StackExchange.Redis;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;

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
                var redisClient = ConnectionMultiplexer.Connect(configurationOptions);
                Database = redisClient.GetDatabase(RedisConfiguration.DbIndex);
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

        byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, o);
            byte[] objectDataAsStream = memoryStream.ToArray();
            return objectDataAsStream;
        }

        T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default;
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using MemoryStream memoryStream = new MemoryStream(stream);
            T result = (T)binaryFormatter.Deserialize(memoryStream);
            return result;
        }
    }
}

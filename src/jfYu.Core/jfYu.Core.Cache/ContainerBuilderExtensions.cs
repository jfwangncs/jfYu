using Autofac;
using jfYu.Core.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace jfYu.Core.Cache
{
    public static class ContainerBuilderExtensions
    {

        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddCache(this ContainerBuilder services, CacheConfig cacheConfig = null)
        {

            var config = cacheConfig ?? AppConfig.Configuration?.GetSection("Cache")?.Get<CacheConfig>() ?? new CacheConfig();
            switch (config.Type)
            {
                case CacheType.Redis:
                    services.Register(q => new RedisCacheService(config)).As<ICacheService>().SingleInstance();
                    break;
                case CacheType.Memory:
                    services.Register(q => new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()), config)).As<ICacheService>().SingleInstance();
                    break;
                default:
                    services.Register(q => new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()), config)).As<ICacheService>().SingleInstance();
                    break;
            }
        }

        /// <summary>
        /// 属性注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddCacheAsProperties(this ContainerBuilder services, CacheConfig cacheConfig = null)
        {
            var config = cacheConfig ?? AppConfig.Configuration?.GetSection("Cache")?.Get<CacheConfig>() ?? new CacheConfig();
            switch (config.Type)
            {
                case CacheType.Redis:
                    services.Register(q => new RedisCacheService(config)).As<ICacheService>().SingleInstance().PropertiesAutowired();
                    break;
                case CacheType.Memory:
                    services.Register(q => new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()), config)).As<ICacheService>().SingleInstance().PropertiesAutowired();
                    break;
                default:
                    services.Register(q => new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()), config)).As<ICacheService>().SingleInstance().PropertiesAutowired();
                    break;
            }
        }
    }
}

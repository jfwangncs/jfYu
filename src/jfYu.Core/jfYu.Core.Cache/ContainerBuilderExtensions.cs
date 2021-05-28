using Autofac;
using jfYu.Core.Common.Configurations;
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
        public static void AddCache(this ContainerBuilder services)
        {
            
            var cacheConfig = AppConfig.Configuration.GetSection("Cache")?.Get<CacheConfig>() ?? new CacheConfig();
            switch (cacheConfig.Type)
            {
                case CacheType.Redis:
                    services.Register(q => new RedisCacheService(cacheConfig)).As<ICache>().SingleInstance();
                    break;
                case CacheType.Memory:
                    services.Register(q => new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()), cacheConfig)).As<ICache>().SingleInstance();
                    break;
                default:
                    services.Register(q => new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()), cacheConfig)).As<ICache>().SingleInstance();
                    break;
            }
        }

        /// <summary>
        /// 属性注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddCacheAsProperties(this ContainerBuilder services)
        {
            var cacheConfig = AppConfig.Configuration.GetSection("Cache")?.Get<CacheConfig>() ?? new CacheConfig();
            switch (cacheConfig.Type)
            {
                case CacheType.Redis:
                    services.Register(q => new RedisCacheService(cacheConfig)).As<ICache>().SingleInstance();
                    break;
                case CacheType.Memory:
                    services.Register(q => new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()), cacheConfig)).As<ICache>().SingleInstance();
                    break;
                default:
                    services.Register(q => new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()), cacheConfig)).As<ICache>().SingleInstance();
                    break;
            }
        }
    }
}

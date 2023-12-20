using Microsoft.Extensions.DependencyInjection;

namespace jfYu.Core.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddCacheService(this IServiceCollection services, bool isRedis = false, string redisConnstr = "")
        {
            if (isRedis)
                services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnstr; });
            else
                services.AddDistributedMemoryCache();

            services.AddScoped<CacheService>();
        }
    }
}

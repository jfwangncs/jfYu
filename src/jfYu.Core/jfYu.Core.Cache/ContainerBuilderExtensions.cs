using Microsoft.Extensions.DependencyInjection;

namespace jfYu.Core.Cache
{
    /// <summary>
    /// 
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// injection
        /// </summary>
        /// <param name="services"></param>
        public static void AddCacheService(this IServiceCollection services, string redisConnstr = "")
        {
            if (string.IsNullOrEmpty(redisConnstr))
                services.AddDistributedMemoryCache();
            else
                services.AddStackExchangeRedisCache(options => { options.Configuration = redisConnstr; });

            services.AddScoped<ICacheService,CacheService>();
        }
    }
}

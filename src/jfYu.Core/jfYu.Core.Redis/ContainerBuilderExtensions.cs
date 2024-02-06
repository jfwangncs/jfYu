using Microsoft.Extensions.DependencyInjection;

namespace jfYu.Core.Redis
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
        public static void AddRedisService(this IServiceCollection services, RedisConfiguration redisConfiguration)
        {
            services.AddSingleton(redisConfiguration);
            services.AddScoped<IRedisService, RedisService>();
        }
    }
}

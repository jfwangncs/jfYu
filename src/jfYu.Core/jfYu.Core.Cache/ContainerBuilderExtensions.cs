using jfYu.Core.Cache.Configurations;
using Microsoft.Extensions.DependencyInjection;
using System;

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
        public static IServiceCollection AddCacheService(this IServiceCollection services, Action<CacheOptions> setupAction)
        {
            ArgumentNullException.ThrowIfNull(setupAction);
            var options = new CacheOptions();
            setupAction(options);
            options.OptionsExtension?.AddServices(services);
            services.AddSingleton(options);
            services.AddSingleton<ICacheService, CacheService>();
            return services;
        }
    }
}

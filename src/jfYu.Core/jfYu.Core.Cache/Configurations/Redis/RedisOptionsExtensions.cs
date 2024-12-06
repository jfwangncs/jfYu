using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace jfYu.Core.Cache.Configurations.Redis
{
    public class RedisOptionsExtensions : IOptionsExtension
    {
        /// <summary>
        /// The configure.
        /// </summary>
        private readonly Action<RedisOptions> _configure;

        public RedisOptionsExtensions(Action<RedisOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.Configure(_configure);
            services.AddStackExchangeRedisCache((Action<RedisCacheOptions>)_configure);
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Linq;

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
            if (redisConfiguration == null || redisConfiguration.EndPoints == null || !redisConfiguration.EndPoints.Any())
                throw new NullReferenceException(nameof(redisConfiguration));
            services.AddSingleton(redisConfiguration);
            var configurationOptions = new ConfigurationOptions()
            {
                Password = redisConfiguration.Password,
                ConnectTimeout = redisConfiguration.Timeout,
                KeepAlive = 60,
                AbortOnConnectFail = false,
                Ssl = redisConfiguration.Ssl,
            };
            foreach (var endPoint in redisConfiguration.EndPoints)
            {
                configurationOptions.EndPoints.Add(endPoint.Host, endPoint.Port);
            }
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configurationOptions));
            services.AddScoped<IRedisService, RedisService>();
        }
    }
}

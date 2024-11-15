using jfYu.Core.Redis.Implementation;
using jfYu.Core.Redis.Interface;
using jfYu.Core.Redis.Serializer;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
        public static IServiceCollection AddRedisService(this IServiceCollection services, Action<RedisConfiguration> setupAction)
        {
            ArgumentNullException.ThrowIfNull(setupAction);

            var options = new RedisConfiguration();
            setupAction(options);

            if (options.EndPoints.Count == 0)
                throw new NullReferenceException(nameof(options));

            var configurationOptions = new ConfigurationOptions()
            {
                Password = options.Password,
                ConnectTimeout = options.Timeout,
                KeepAlive = 60,
                AbortOnConnectFail = false,
                Ssl = options.Ssl,
            };

            foreach (var endPoint in options.EndPoints)
            {
                configurationOptions.EndPoints.Add(endPoint.Host, endPoint.Port);
            }

            services.Configure(setupAction);

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configurationOptions));

            services.AddScoped<IRedisService, RedisService>();

            // Check if ISerializer is already registered
            if (!services.Any(service => service.ServiceType == typeof(ISerializer)))
            {
                // If not, use Newtonsoft.Json as the default serializer
                services.UsingNewtonsoft();
            }

            return services;
        }

        public static IServiceCollection UsingNewtonsoft(this IServiceCollection services, Action<JsonSerializerSettings>? setupAction = null)
        {
            var options = new JsonSerializerSettings();
            setupAction?.Invoke(options);
            services.AddSingleton<ISerializer>(new NewtonsoftSerializer(options));

            return services;
        }
        public static IServiceCollection UsingMsgPack(this IServiceCollection services, Action<MessagePackSerializerOptions>? setupAction = null)
        {
            var options = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);
            setupAction?.Invoke(options);
            services.AddSingleton<ISerializer>(new MsgPackObjectSerializer(options));
            return services;
        }
    }
}

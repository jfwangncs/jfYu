using jfYu.Core.Redis.Implementation;
using jfYu.Core.Redis.Interface;
using jfYu.Core.Redis.Options;
using jfYu.Core.Redis.Serializer.MessagePack;
using jfYu.Core.Redis.Serializer.Newtonsoft;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

namespace jfYu.Core.Redis.Extensions
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
        public static IServiceCollection AddRedisService(this IServiceCollection services, Action<RedisOptions>? setupAction)
        {
            ArgumentNullException.ThrowIfNull(setupAction);

            var options = new RedisOptions();
            setupAction.Invoke(options);

            if (options.EndPoints.Count == 0)
                throw new NullReferenceException(nameof(options));

            var configurationOptions = new ConfigurationOptions()
            {
                Password = options.Password,
                ConnectTimeout = options.Timeout,
                KeepAlive = 60,
                AbortOnConnectFail = false,
                Ssl = options.SSL,
            };

            foreach (var endPoint in options.EndPoints)
            {
                configurationOptions.EndPoints.Add(endPoint.Host, endPoint.Port);
            }

            services.Configure(setupAction);

            services.AddSingleton(sp => new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configurationOptions)).Value);

            services.AddScoped<IRedisService, RedisService>();

            // Check if ISerializer is already registered
            if (options.SerializerOptions == null)
                // If not, use Newtonsoft.Json as the default serializer
                new NewtonsoftOptionsExtension(null).AddServices(services);
            else
                // If it is, use the registered serializer
                options.SerializerOptions.AddServices(services);

            return services;
        }

        public static void UsingNewtonsoft(this RedisOptions services, Action<JsonSerializerSettings>? setupAction = null)
        {
            services.SerializerOptions = new NewtonsoftOptionsExtension(setupAction);
        }
        public static void UsingMsgPack(this RedisOptions services, Action<MessagePackSerializerOptions>? setupAction = null)
        {
            services.SerializerOptions = new MessagePackOptionsExtension(setupAction);
        }
    }
}
